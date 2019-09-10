namespace MvcForum.Web.Controllers
{
    using Core.ExtensionMethods;
    using MvcForum.Core;
    using MvcForum.Core.Constants;
    using MvcForum.Core.Interfaces;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Core.Models.Entities;
    using MvcForum.Core.Models.Enums;
    using MvcForum.Core.Models.General;
    using MvcForum.Core.Utilities;
    using MvcForum.Web.ViewModels.Mapping;
    using MvcForum.Web.ViewModels.Topic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    public class EventosController : BaseController
    {
        public int TipoEvento
        {
            get
            {
                if (TempData["TipoEvento"] == null)
                    TempData["TipoEvento"] = -1;

                return (int)TempData["TipoEvento"];
            }
            set
            {
                TempData["TipoEvento"] = value;
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
        private readonly IMembershipUserTopicInterestService _membershipUserTopicInterestService;
        private readonly IMembershipUserTopicIGoService _membershipUserTopicIGoService;

        public EventosController(IPostService postService, ITopicService topicService, ICategoryService categoryService,
                                     IFavouriteService favouriteService, INotificationService notificationService, IPollService pollService,
                                     IVoteService voteService,
                                     ILoggingService loggingService, IMembershipService membershipService, IRoleService roleService,
                                     IMembershipUserTopicIGoService membershipUserTopicIGoService,
                                     IMembershipUserTopicInterestService membershipUserTopicInterestService,
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
            _membershipUserTopicInterestService = membershipUserTopicInterestService;
            _membershipUserTopicIGoService = membershipUserTopicIGoService;
        }

        // GET: VerAnuncios
        public ActionResult Index(int? tipoEvento)
        {
            TipoEvento = tipoEvento.HasValue ? tipoEvento.Value : -1;

            return View();
        }

        public virtual ActionResult ShowEventos(int? p)
        {
            MembershipUser loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            MembershipUser membershipUser = base.User.GetMembershipUser(MembershipService);
            MembershipRole role = membershipUser.GetRole(RoleService);
            List<Category> allowedCategories = _categoryService.GetAllowedCategories(role);

            // Recupera as configurações padrão do sistema
            Settings settings = SettingsService.GetSettings();

            // Recupera o indice da página, caso ele exista
            int pageIndex = p ?? 1;
            

            PaginatedList<Topic> result = null;

            if (!string.IsNullOrEmpty(Request.QueryString["valor"]))
            {
                var valores = Request.QueryString["valor"];

                valores = valores.Substring(valores.LastIndexOf('=') + 1);

                var estado = valores.Split('-')[0];
                var cidade = valores.Split('-')[1];                               

                
                if (!string.IsNullOrEmpty(estado) && !string.IsNullOrEmpty(cidade))
                {
                    result = Task.Run(() => _topicService.GetEventosByRegiao(pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize, cidade, estado)).Result;
                }else if(string.IsNullOrEmpty(cidade))
                    result = Task.Run(() => _topicService.GetEventosByRegiao(pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize, string.Empty, estado)).Result;

            }
            else { 

                if (loggedOnReadOnlyUser != null && TipoEvento != -1 )
                {
                    switch (TipoEvento)
                    {
                        case 0:
                            var interesse = _membershipUserTopicInterestService.GetByUser(membershipUser.Id).Select(x => x.IdTopic).ToList();

                            result = Task.Run(() => _topicService.GetAllEventosByID(pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize,interesse)).Result;
                            break;
                        case 1:
                            var euVou = _membershipUserTopicIGoService.GetByUser(membershipUser.Id).Select(x => x.IdTopic).ToList();

                            result = Task.Run(() => _topicService.GetAllEventosByID(pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize, euVou)).Result;
                            break;
                    }
                }
                else 
                    result = Task.Run(() => _topicService.GetEventos(pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize)).Result;

                if (result == null)
                    result = new PaginatedList<Topic>(null, pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize);
            }
            // Cria a classe do post indicado
            List<TopicViewModel> topics = ViewModelMapping.CreateTopicViewModels(result, RoleService, role, membershipUser, allowedCategories, settings,
                                                                _postService, _notificationService, _pollService, _voteService, _favouriteService);

            
            ActiveTopicsViewModel model = new ActiveTopicsViewModel
            {
                Topics = topics,
                PageIndex = pageIndex,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            };

            return PartialView(model);
        }
        [ChildActionOnly]
        public ActionResult Calendario()
        {
            var dadosEvento = _topicService.GetEventosData();

            ActiveTopicsViewModel model = new ActiveTopicsViewModel
            {
                DatasEvento = dadosEvento
            };

            return PartialView("_Calendario", model);
        }

        private List<Category> AllowedCreateCategories(MembershipRole loggedOnUsersRole)
        {
            List<Category> allowedAccessCategories = _categoryService.GetAllowedCategories(loggedOnUsersRole);
            List<Category> allowedCreateTopicCategories =
                _categoryService.GetAllowedCategories(loggedOnUsersRole,
                    ForumConfiguration.Instance.PermissionCreateTopics);
            IEnumerable<Guid> allowedCreateTopicCategoryIds = allowedCreateTopicCategories.Select(x => x.Id);
            if (allowedAccessCategories.Any())
            {
                allowedAccessCategories.RemoveAll(x => allowedCreateTopicCategoryIds.Contains(x.Id));
                allowedAccessCategories.RemoveAll(x =>
                    loggedOnUsersRole.RoleName != Constants.AdminRoleName && x.IsLocked);
            }
            return allowedAccessCategories;
        }


        [HttpPost]
        public JsonResult TenhoInteresseEvent(Guid id)
        {
            // Get the post
            Post post = _postService.Get(id);

            if (post != null)
            {
                MembershipUser loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
                MembershipRole loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);
                List<Category> allowedAccessCategories = AllowedCreateCategories(loggedOnUsersRole);

                if (allowedAccessCategories.Any() && loggedOnReadOnlyUser.DisablePosting != true)
                {
                    IEnumerable<MembershipUserTopicInterest> novo = _membershipUserTopicInterestService.GetByUser(loggedOnReadOnlyUser.Id).Where(x => x.IdTopic.Equals(post.Topic.Id));
                    try
                    {
                        if (!novo.Any())
                        {
                            //new item
                            MembershipUserTopicInterest memberInterest = new MembershipUserTopicInterest
                            {
                                IdTopic = post.Topic.Id,
                                IdUser = loggedOnReadOnlyUser.Id
                            };
                            MembershipUserTopicInterest interesse = _membershipUserTopicInterestService.Add(memberInterest);
                            _membershipUserTopicInterestService.SaveChanges();
                            return Json("Interesse incluído com sucesso!");
                        }
                        else
                        {
                            _membershipUserTopicInterestService.Delete(novo.FirstOrDefault());
                            _membershipUserTopicInterestService.SaveChanges();
                            return Json("Interesse removido com sucesso!");
                        }
                    }
                    catch { return Json("Erro. Por favor, tente novamente!"); }


                }
            }
            return Json("Você precisa estar logado para realizar esta ação.");
        }

        [HttpPost]
        public JsonResult EuVouEvent(Guid id)
        {
            // Get the post
            Post post = _postService.Get(id);

            if (post != null)
            {
                MembershipUser loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
                MembershipRole loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);
                List<Category> allowedAccessCategories = AllowedCreateCategories(loggedOnUsersRole);

                if (allowedAccessCategories.Any() && loggedOnReadOnlyUser.DisablePosting != true)
                {
                    IEnumerable<MemberShipUserTopicIGo> novo = _membershipUserTopicIGoService.GetByUser(loggedOnReadOnlyUser.Id).Where(x => x.IdTopic.Equals(post.Topic.Id));
                    try
                    {
                        if (!novo.Any())
                        {
                            //new item
                            MemberShipUserTopicIGo euVou = new MemberShipUserTopicIGo
                            {
                                IdTopic = post.Topic.Id,
                                IdUser = loggedOnReadOnlyUser.Id
                            };
                            MemberShipUserTopicIGo vou = _membershipUserTopicIGoService.Add(euVou);
                            _membershipUserTopicInterestService.SaveChanges();

                            //TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                            //{
                            //    Message = "Eu vou incluído com sucesso!",
                            //    MessageType = GenericMessages.success
                            //};
                            return Json("Eu vou incluído com sucesso!");
                        }
                        else
                        {
                            _membershipUserTopicIGoService.Delete(novo.FirstOrDefault());
                            _membershipUserTopicIGoService.SaveChanges();
                            return Json("Eu vou removido com sucesso!");

                            //TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                            //{
                            //    Message = "Eu vou removido com sucesso!",
                            //    MessageType = GenericMessages.success
                            //};                            
                        }


                    }
                    catch
                    {
                        //TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                        //{
                        //    Message = "Erro. Por favor, tente novamente!!",
                        //    MessageType = GenericMessages.danger
                        //};
                        return Json("Erro. Por favor, tente novamente!");

                    }


                }
            }
            else
            {
                //TempData["Messagem"] = "Erro. Por favor, tente novamente!";
                //TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                //{
                //    Message = "Erro. Por favor, tente novamente!!",
                //    MessageType = GenericMessages.danger
                //};
                return Json("Erro. Por favor, tente novamente!");
            }
            return Json("Você precisa estar logado para realizar esta ação.");
        }

        public virtual async Task<ActionResult> TenhoInteresse(string slug, int? p)
        {
            // Set the page index
            int pageIndex = p ?? 1;

            MembershipUser loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            MembershipRole loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);

            // Get the topic
            Topic topic = _topicService.GetTopicBySlug(slug);

            if (topic != null)
            {
                Settings settings = SettingsService.GetSettings();

                string sortQuerystring = Request.QueryString[Constants.PostOrderBy];
                PostOrderBy orderBy = !string.IsNullOrWhiteSpace(sortQuerystring)
                    ? EnumUtils.ReturnEnumValueFromString<PostOrderBy>(sortQuerystring)
                    : PostOrderBy.Standard;

                int amountPerPage = settings.PostsPerPage;

                if (sortQuerystring == Constants.AllPosts)
                {
                    amountPerPage = int.MaxValue;
                }

                PaginatedList<Core.Models.Entities.Post> posts = null;
                if (loggedOnUsersRole.RoleName != "Editores")
                {
                    posts = await _postService.GetPagedPostsByTopic(pageIndex, amountPerPage, int.MaxValue, topic.Id, orderBy);
                }
                else
                {
                    posts = await _postService.GetPagedPostsByTopicAndUser(pageIndex, amountPerPage, int.MaxValue, topic.Id, orderBy);
                }

                // Get the topic starter post
                Post starterPost = _postService.GetTopicStarterPost(topic.Id);

                // Get the permissions for the category that this topic is in
                PermissionSet permissions = RoleService.GetPermissions(topic.Category, loggedOnUsersRole);

                // If this user doesn't have access to this topic then
                // redirect with message
                if (permissions[ForumConfiguration.Instance.PermissionDenyAccess].IsTicked)
                {
                    return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
                }

                // Set editor permissions
                ViewBag.ImageUploadType = permissions[ForumConfiguration.Instance.PermissionInsertEditorImages].IsTicked
                    ? "forumimageinsert"
                    : "image";

                List<Guid> postIds = posts.Select(x => x.Id).ToList();

                Dictionary<Guid, List<Vote>> votes = _voteService.GetVotesByPosts(postIds);

                Dictionary<Guid, List<Favourite>> favourites = _favouriteService.GetAllPostFavourites(postIds);

                int interesses = _membershipUserTopicInterestService.GetByTopic(topic.Id).Count();
                int qtdVou = _membershipUserTopicIGoService.GetByTopic(topic.Id).Count();



                TopicViewModel viewModel = ViewModelMapping.CreateTopicViewModel(topic, permissions, posts, postIds,
                    starterPost, posts.PageIndex, posts.TotalCount, posts.TotalPages, loggedOnReadOnlyUser,
                    settings, _notificationService, _pollService, votes, favourites, true);

                // If there is a quote querystring
                string quote = Request["quote"];
                if (!string.IsNullOrWhiteSpace(quote))
                {
                    try
                    {
                        // Got a quote
                        Post postToQuote = _postService.Get(new Guid(quote));
                        viewModel.QuotedPost = postToQuote.PostContent;
                        viewModel.ReplyTo = postToQuote.Id;
                        viewModel.ReplyToUsername = postToQuote.User.UserName;
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                    }
                }

                string reply = Request["reply"];
                if (!string.IsNullOrWhiteSpace(reply))
                {
                    try
                    {
                        // Set the reply
                        Post toReply = _postService.Get(new Guid(reply));
                        viewModel.ReplyTo = toReply.Id;
                        viewModel.ReplyToUsername = toReply.User.UserName;
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                    }
                }

                bool updateDatabase = false;

                if (User.Identity.IsAuthenticated)
                {
                    IEnumerable<MembershipUserTopicInterest> dataInteresse = _membershipUserTopicInterestService.GetByUser(loggedOnReadOnlyUser.Id).Where(x => x.IdTopic.Equals(topic.Id));
                    IEnumerable<MemberShipUserTopicIGo> dataEuVou = _membershipUserTopicIGoService.GetByUser(loggedOnReadOnlyUser.Id).Where(x => x.IdTopic.Equals(topic.Id));

                    viewModel.IsInteresse = dataInteresse.Any();
                    viewModel.IsEuVou = dataEuVou.Any();

                }
                else
                {
                    viewModel.IsInteresse = false;
                    viewModel.IsEuVou = false;

                }

                // User has permission lets update the topic view count
                // but only if this topic doesn't belong to the user looking at it
                bool addView = !(User.Identity.IsAuthenticated && loggedOnReadOnlyUser.Id == topic.User.Id);

                if (addView)
                {
                    updateDatabase = true;
                }

                // Check the poll - To see if it has one, and whether it needs to be closed.
                if (viewModel.Poll?.Poll?.ClosePollAfterDays != null &&
                    viewModel.Poll.Poll.ClosePollAfterDays > 0 &&
                    !viewModel.Poll.Poll.IsClosed)
                {
                    // Check the date the topic was created
                    DateTime endDate =
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
                    {
                        topic.Views = topic.Views + 1;
                    }

                    try
                    {
                        Context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        LoggingService.Error(ex);
                    }
                }

                viewModel.InteresseCount = interesses;
                viewModel.EuVouCount = qtdVou;

                return View(viewModel);
            }

            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.GenericMessage"));
        }

        private CreateEditTopicEventViewModel PrePareCreateEditTopicViewModel(List<Category> allowedCategories)
        {
            var userIsAdmin = User.IsInRole(Constants.AdminRoleName);
            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);
            var permissions = RoleService.GetPermissions(null, loggedOnUsersRole);
            var canInsertImages = userIsAdmin;
            if (!canInsertImages)
            {
                canInsertImages = permissions[ForumConfiguration.Instance.PermissionInsertEditorImages].IsTicked;
            }
            return new CreateEditTopicEventViewModel
            {
                //SubscribeToTopic = true,
                //Categories = _categoryService.GetBaseSelectListCategories(allowedCategories),
                //OptionalPermissions = new CheckCreateTopicPermissions
                //{
                //    CanLockTopic = userIsAdmin,
                //    CanStickyTopic = userIsAdmin,
                //    CanUploadFiles = userIsAdmin,
                //    CanCreatePolls = userIsAdmin,
                //    CanInsertImages = canInsertImages,
                //    CanCreateTags = userIsAdmin
                //},
                //PollAnswers = new List<PollAnswer>(),
                //IsTopicStarter = true,
                //PollCloseAfterDays = 0
            };
        }

        [Authorize]
        public virtual ActionResult Novo()
        {
            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);
            var allowedAccessCategories = AllowedCreateCategories(loggedOnUsersRole);

            if (allowedAccessCategories.Any() && loggedOnReadOnlyUser.DisablePosting != true)
            {
                var viewModel = PrePareCreateEditTopicViewModel(allowedAccessCategories);
                return View(viewModel);
            }
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

        [Authorize]
        public virtual ActionResult Editar(Guid id)
        {
            // Get the post
            var post = _postService.Get(id);

            // Get the topic
            var topic = post.Topic;

            // Get the current logged on user
            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnloggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);

            // get the users permissions
            var permissions = RoleService.GetPermissions(topic.Category, loggedOnloggedOnUsersRole);

            // Is the user allowed to edit this post
            if (post.User.Id == loggedOnReadOnlyUser.Id ||
                permissions[ForumConfiguration.Instance.PermissionEditPosts].IsTicked)
            {
                // Get the allowed categories for this user
                var allowedAccessCategories = _categoryService.GetAllowedCategories(loggedOnloggedOnUsersRole);
                var allowedCreateTopicCategories =
                    _categoryService.GetAllowedCategories(loggedOnloggedOnUsersRole,
                        ForumConfiguration.Instance.PermissionCreateTopics);
                var allowedCreateTopicCategoryIds = allowedCreateTopicCategories.Select(x => x.Id);

                // If this user hasn't got any allowed cats OR they are not allowed to post then abandon
                if (allowedAccessCategories.Any() && loggedOnReadOnlyUser.DisablePosting != true)
                {
                    

                    var viewModel = new CreateEditTopicEventViewModel
                    {
                        Name = topic.Name,
                        Content = topic.Posts.FirstOrDefault().PostContent,
                        Id = topic.Id,
                        TopicId = topic.Id,
                        LocalEvento = topic.LocalEvento,
                        DataEventoInicio = topic.DataEventoInicio,
                        DataEventoFim = topic.DataEventoFim,
                        HoraInicio = topic.HoraEventoInicio,
                        HoraFim = topic.HoraEventoFim,                        
                        LinkExternoEvento = topic.LinkExternoEvento,
                        CidadeEvento= topic.CidadeEvento,
                        EstadoEvento = topic.EstadoEvento
                    };

                    



                    // Now check if this is a topic starter, if so add the rest of the field
                    if (post.IsTopicStarter)
                    {
                        // Remove all Categories that don't have create topic permission
                        allowedAccessCategories.RemoveAll(x => allowedCreateTopicCategoryIds.Contains(x.Id));

                        // See if this user is subscribed to this topic
                        var topicNotifications =
                            _notificationService.GetTopicNotificationsByUserAndTopic(loggedOnReadOnlyUser, topic);

                        // Populate the properties we can
                        //viewModel.IsLocked = topic.IsLocked;
                        //viewModel.IsSticky = topic.IsSticky;
                        //viewModel.IsTopicStarter = post.IsTopicStarter;
                        //viewModel.SubscribeToTopic = topicNotifications.Any();
                        //viewModel.Categories =
                        //    _categoryService.GetBaseSelectListCategories(allowedAccessCategories);

                        // Tags - Populate from the topic
                        //if (topic.Tags.Any())
                        //{
                        //    viewModel.Tags = string.Join<string>(",", topic.Tags.Select(x => x.Tag));
                        //}

                        // Populate the poll answers
                        //if (topic.Poll != null && topic.Poll.PollAnswers.Any())
                        //{
                        //    // Has a poll so add it to the view model
                        //    viewModel.PollAnswers = topic.Poll.PollAnswers;
                        //    viewModel.PollCloseAfterDays = topic.Poll.ClosePollAfterDays ?? 0;
                        //}

                        //// It's a topic
                        //viewModel.IsPostEdit = false;
                    }

                    // Return the edit view
                    return View(viewModel);
                }
            }

            // If we get here the user has no permission to try and edit the post
            return ErrorToHomePage(LocalizationService.GetResourceString("Errors.NoPermission"));
        }

    }
}