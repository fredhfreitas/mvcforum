namespace MvcForum.Web.Controllers
{
    using MvcForum.Core;
    using MvcForum.Core.Interfaces;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Web.ViewModels.Mapping;
    using MvcForum.Web.ViewModels.Topic;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.ExtensionMethods;
    using MvcForum.Web.ViewModels.Category;
    using System.Linq;
    using MvcForum.Core.Constants;
    using MvcForum.Core.Models.Enums;
    using MvcForum.Core.Utilities;
    using MvcForum.Core.Models.General;
    using System;
    using MvcForum.Core.Models.Entities;
    using Newtonsoft.Json;
    using MvcForum.Web.ViewModels.CategoriaAnuncio;
    using System.Collections.Generic;

    public partial class VerAnunciosController : BaseController
    {
        public int TipoAnuncio
        {
            get
            {
                if (TempData["TipoAnuncio"] == null)
                    TempData["TipoAnuncio"] = -1;

                return (int)TempData["TipoAnuncio"];
            }
            set
            {
                TempData["TipoAnuncio"] = value;
            }
        }

        private readonly IPostService _postService;
        private readonly ITopicService _topicService;
        private readonly ICategoryService _categoryService;
        private readonly IFavouriteService _favouriteService;
        private readonly INotificationService _notificationService;
        private readonly IPollService _pollService;
        private readonly IVoteService _voteService;
        private readonly IRoleService _roleService;

        public VerAnunciosController(IPostService postService, ITopicService topicService, ICategoryService categoryService,
                                     IFavouriteService favouriteService, INotificationService notificationService, IPollService pollService,
                                     IVoteService voteService,
                                     ILoggingService loggingService, IMembershipService membershipService, IRoleService roleService,
                                     ILocalizationService localizationService, ISettingsService settingsService,
                                     ICacheService cacheService, IMvcForumContext context)
            : base(loggingService, membershipService, localizationService, roleService,
                   settingsService, cacheService, context)
        {
            _postService = postService;
            _topicService = topicService;

            _categoryService = categoryService;
            _favouriteService = favouriteService;
            _notificationService = notificationService;
            _pollService = pollService;
            _voteService = voteService;

            _roleService = roleService;
        }

        // GET: VerAnuncios
        public ActionResult Index(int? tipoAnuncio)
        {
            TipoAnuncio = tipoAnuncio.HasValue ? tipoAnuncio.Value : -1;

            return View();
        }

        public virtual ActionResult FotosAnuncios(int? p)
        {
            var membershipUser = base.User.GetMembershipUser(MembershipService);
            var role = membershipUser.GetRole(RoleService);
            var allowedCategories = _categoryService.GetAllowedCategories(role);

            // Recupera as configurações padrão do sistema
            var settings = SettingsService.GetSettings();

            // Recupera o indice da página, caso ele exista
            int pageIndex = p ?? 1;

            PaginatedList<Topic> result = null;

            if (TipoAnuncio != -1)
                switch (TipoAnuncio)
                {
                    case 0:
                        result = Task.Run(() => _topicService.GetAnunciosNovos(pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize)).Result;
                        break;
                    case 1:
                        result = Task.Run(() => _topicService.GetAnunciosUsados(pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize)).Result;
                        break;
                    case 2:
                        result = Task.Run(() => _topicService.GetAnunciosTrocas(pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize)).Result;
                        break;
                    case 3:
                        result = Task.Run(() => _topicService.GetAnunciosPorTipo(pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize, "Retro")).Result;
                        break;
                    case 4:
                        result = Task.Run(() => _topicService.GetAnunciosPorTipo(pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize, "Acessorio")).Result;
                        break;
                    case 5:
                        result = Task.Run(() => _topicService.GetAnunciosPorTipo(pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize, "Servico")).Result;
                        break;
                }
            else
                result = Task.Run(() => _topicService.GetAnuncios(pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize)).Result;

            // Cria a classe do post indicado
            var topics = ViewModelMapping.CreateTopicViewModels(result, RoleService, role, membershipUser, allowedCategories, settings,
                                                                _postService, _notificationService, _pollService, _voteService, _favouriteService);

            var model = new ActiveTopicsViewModel
            {
                Topics = topics,
                PageIndex = pageIndex,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            };

            return PartialView(model);
        }

        [ChildActionOnly]
        public virtual PartialViewResult ListarFiltrosSideMenu(string value)
        {
            // Recupera o post pelo ID
            var post = _postService.Get(Guid.Parse(value));

            var conteudoJson = post.PostContent;

            // Recupera o conteúdo do post em JSON que esta em uma lista
            var conteudoPost = JsonConvert.DeserializeObject<List<CategoriaAnuncioViewModel>>(conteudoJson);

            return PartialView(conteudoPost);
        }

        public virtual async Task<ActionResult> TenhoInteresse(string slug, int? p)
        {
            // Set the page index
            var pageIndex = p ?? 1;

            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);

            // Get the topic
            var topic = _topicService.GetTopicBySlug(slug);

            if (topic != null)
            {
                var settings = SettingsService.GetSettings();

                var sortQuerystring = Request.QueryString[Constants.PostOrderBy];
                var orderBy = !string.IsNullOrWhiteSpace(sortQuerystring)
                    ? EnumUtils.ReturnEnumValueFromString<PostOrderBy>(sortQuerystring)
                    : PostOrderBy.Standard;

                var amountPerPage = settings.PostsPerPage;

                if (sortQuerystring == Constants.AllPosts)
                    amountPerPage = int.MaxValue;

                PaginatedList<Core.Models.Entities.Post> posts = null;
                if (loggedOnUsersRole.RoleName != "Editores")
                    posts = await _postService.GetPagedPostsByTopic(pageIndex, amountPerPage, int.MaxValue, topic.Id, orderBy);
                else
                    posts = await _postService.GetPagedPostsByTopicAndUser(pageIndex, amountPerPage, int.MaxValue, topic.Id, orderBy);

                // Get the topic starter post
                var starterPost = _postService.GetTopicStarterPost(topic.Id);

                // Get the permissions for the category that this topic is in
                var permissions = RoleService.GetPermissions(topic.Category, loggedOnUsersRole);

                // If this user doesn't have access to this topic then
                // redirect with message
                if (permissions[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
                    return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));

                // Set editor permissions
                ViewBag.ImageUploadType = permissions[ForumConfiguration.Instance.PermissionInsertEditorImages].IsTicked
                    ? "forumimageinsert"
                    : "image";

                var postIds = posts.Select(x => x.Id).ToList();

                var votes = _voteService.GetVotesByPosts(postIds);

                var favourites = _favouriteService.GetAllPostFavourites(postIds);

                var viewModel = ViewModelMapping.CreateTopicViewModel(topic, permissions, posts, postIds,
                    starterPost, posts.PageIndex, posts.TotalCount, posts.TotalPages, loggedOnReadOnlyUser,
                    settings, _notificationService, _pollService, votes, favourites, true);

                // If there is a quote querystring
                var quote = Request["quote"];
                if (!string.IsNullOrWhiteSpace(quote))
                {
                    try
                    {
                        // Got a quote
                        var postToQuote = _postService.Get(new Guid(quote));
                        viewModel.QuotedPost = postToQuote.PostContent;
                        viewModel.ReplyTo = postToQuote.Id;
                        viewModel.ReplyToUsername = postToQuote.User.UserName;
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                    }
                }

                var reply = Request["reply"];
                if (!string.IsNullOrWhiteSpace(reply))
                {
                    try
                    {
                        // Set the reply
                        var toReply = _postService.Get(new Guid(reply));
                        viewModel.ReplyTo = toReply.Id;
                        viewModel.ReplyToUsername = toReply.User.UserName;
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                    }
                }

                var updateDatabase = false;

                // User has permission lets update the topic view count
                // but only if this topic doesn't belong to the user looking at it
                var addView = !(User.Identity.IsAuthenticated && loggedOnReadOnlyUser.Id == topic.User.Id);

                if (addView)
                    updateDatabase = true;

                // Check the poll - To see if it has one, and whether it needs to be closed.
                if (viewModel.Poll?.Poll?.ClosePollAfterDays != null &&
                    viewModel.Poll.Poll.ClosePollAfterDays > 0 &&
                    !viewModel.Poll.Poll.IsClosed)
                {
                    // Check the date the topic was created
                    var endDate =
                        viewModel.Poll.Poll.DateCreated.AddDays((int)viewModel.Poll.Poll.ClosePollAfterDays);
                    if (DateTime.UtcNow > endDate)
                    {
                        topic.Poll.IsClosed = true;
                        viewModel.Topic.Poll.IsClosed = true;
                        updateDatabase = true;
                    }
                }

                if (!BotUtils.UserIsBot() && updateDatabase)
                {
                    if (addView)
                        topic.Views = topic.Views + 1;

                    try
                    {
                        Context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                    }
                }

                return View(viewModel);
            }

            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
        }
    }
}