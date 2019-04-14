namespace MvcForum.Web.Controllers
{
    using MvcForum.Core.Interfaces;
    using MvcForum.Core.Interfaces.Services;
    using Newtonsoft.Json;
    using MvcForum.Core.Services;
    using MvcForum.Web.ViewModels.Category;
    using MvcForum.Web.ViewModels.Mapping;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Threading.Tasks;

    using Application.CustomActionResults;
    using Core;
    using Core.ExtensionMethods;
    using Core.Models.Entities;
    using Core.Models.Enums;
    using Core.Models.General;
    using ViewModels.Breadcrumb;
    using ViewModels.CategoriaAnuncio;
    using MvcForum.Web.ViewModels.Post;
    using MvcForum.Web.ViewModels.Topic;

    public partial class CategoriaAnuncioController : BaseController
    {
        private readonly IPostService _postService;
        private readonly ITopicService _topicService;
        private readonly ICategoryService _categoryService;
        private readonly IFavouriteService _favouriteService;
        private readonly INotificationService _notificationService;
        private readonly IPollService _pollService;
        private readonly IVoteService _voteService;

        public CategoriaAnuncioController(IPostService postService, ITopicService topicService, ICategoryService categoryService,
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

            _postService = postService;
            _topicService = topicService;
            _categoryService = categoryService;
            _favouriteService = favouriteService;
            _notificationService = notificationService;
            _pollService = pollService;
            _voteService = voteService;
        }

        // GET: CategoriaAnuncio
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public PartialViewResult ListarCategoriasAnuncio(string value)
        {
            // Recupera o post pelo ID
            var post = _postService.Get(Guid.Parse(value));

            var conteudoJson = post.PostContent;

            // Recupera o conteúdo do post em JSON que esta em uma lista
            var listaConteudoPost = JsonConvert.DeserializeObject<List<CategoriaAnuncioViewModel>>(conteudoJson);

            // Recupera a quantidade de Itens por post
            foreach (var conteudoPost in listaConteudoPost)
            {
                conteudoPost.QuantidadeAnuncio = _topicService.GetQuantidadeTipoAnuncios(conteudoPost.NomeTitulo);
            }

            return PartialView(listaConteudoPost);
        }

        [ChildActionOnly]
        public virtual ActionResult AnunciosMaisVistos(int? p, int Values)
        {
            var membershipUser = base.User.GetMembershipUser(MembershipService);
            var role = membershipUser.GetRole(RoleService);
            var allowedCategories = _categoryService.GetAllowedCategories(role);

            // Recupera as configurações padrão do sistema
            var settings = SettingsService.GetSettings();

            // Recupera o indice da página, caso ele exista
            int pageIndex = p ?? 1;

            // Recupera os anuncios mais vistos
            var result = Task.Run(() => _topicService.GetAnunciosMaisVistos(pageIndex, settings.TopicsPerPage, ForumConfiguration.Instance.ActiveTopicsListSize)).Result;

            // Cria a classe do post indicado
            var topics  = ViewModelMapping.CreateTopicViewModels((Values == 0) ? result : result.Take(Values).ToList(), RoleService, role, 
                                                                 membershipUser, allowedCategories, settings, _postService, _notificationService, 
                                                                 _pollService, _voteService, _favouriteService);

            var model = new ActiveTopicsViewModel
            {
                Topics = topics,
                PageIndex = pageIndex,
                TotalCount = (Values == 4) ? 0 : result.TotalCount,
                TotalPages = result.TotalPages
            };

            return PartialView(model);
        }
    }
}