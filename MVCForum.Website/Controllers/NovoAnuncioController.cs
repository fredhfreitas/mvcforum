namespace MvcForum.Web.Controllers
{
    using Core.ExtensionMethods;
    using Core.Models.Entities;
    using MvcForum.Core.Interfaces;
    using MvcForum.Core.Interfaces.Services;
    using MvcForum.Web.Models;
    using MvcForum.Web.ViewModels.Topic;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;
    using MvcForum.Core;
    using MvcForum.Core.Constants;
    using MvcForum.Web.ViewModels.ExtensionMethods;
    using MvcForum.Web.ViewModels;
    using MvcForum.Core.Models.General;
    using System.Threading.Tasks;
    using System;

    public partial class NovoAnuncioController : BaseController
    {
        public ICategoryService _categoryService;
        public ITopicService _topicService;

        public NovoAnuncioController(ILoggingService loggingService, IMembershipService membershipService, IRoleService roleService,
                                     ITopicService topicService, ICategoryService categoryService, ILocalizationService localizationService,
                                     ISettingsService settingsService, ICacheService cacheService, IMvcForumContext context)
                                    : base(loggingService, membershipService, localizationService, roleService, settingsService, cacheService, context)
        {
            _topicService = topicService;
            _categoryService = categoryService;
        }

        public ActionResult Index()
        {
            var usuario = User.GetMembershipUser(MembershipService);

            if (usuario == null)
                return RedirectToAction("Index", "Home");

            PreencherUsuario(usuario);

            return View();
        }

        [HttpPost]
        public virtual async Task<ActionResult> Index(NovoAnuncioViewModel model)
        {
            // Get the user and roles
            var loggedOnUser = User.GetMembershipUser(MembershipService, false);
            var loggedOnUsersRole = loggedOnUser.GetRole(RoleService);

            // Como inicialmente vamos voltar para a mesma página, estou carregand as View bags novamente
            PreencherUsuario(loggedOnUser);

            // Cria o objeto de TopicViewModel
            var topicViewModel = new CreateEditTopicViewModel();
            topicViewModel.Category = model.CategoriaId;

            topicViewModel.Name = model.TituloAnuncio;
            topicViewModel.Files = model.Imagem;
            topicViewModel.Content = model.Observacao;

            // ID do Usuário Logado
            model.Usuario.IdUsuarioLogado = loggedOnUser.Id;

            // Get the category
            var category = _categoryService.Get(topicViewModel.Category);

            // First check this user is allowed to create topics in this category
            var permissions = RoleService.GetPermissions(category, loggedOnUsersRole);

            // Now we have the category and permissionSet - Populate the optional permissions 
            // This is just in case the viewModel is return back to the view also sort the allowedCategories
            topicViewModel.OptionalPermissions = GetCheckCreateTopicPermissions(permissions);

            topicViewModel.Categories = _categoryService.GetBaseSelectListCategories(AllowedCreateCategories(loggedOnUsersRole));

            topicViewModel.IsTopicStarter = true;

            if (topicViewModel.PollAnswers == null)
                topicViewModel.PollAnswers = new List<PollAnswer>();

            if (ModelState.IsValid)
            {
                // Map the new topic (Pass null for new topic)
                var topic = topicViewModel.ToTopic(category, loggedOnUser, null);

                CarregarCamposTopicViewModel(topic, model);

                // Run the create pipeline
                var createPipeLine = await _topicService.Create(topic, topicViewModel.Files, topicViewModel.Tags, topicViewModel.SubscribeToTopic,
                                                                topicViewModel.Content, null);

                if (createPipeLine.Successful == false)
                {
                    // TODO - Not sure on this?
                    // Remove the topic if unsuccessful, as we may have saved some items.
                    await _topicService.Delete(createPipeLine.EntityToProcess);

                    // Tell the user the topic is awaiting moderation
                    ModelState.AddModelError(string.Empty, createPipeLine.ProcessLog.FirstOrDefault());

                    return View(model);
                }

                if (createPipeLine.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.Moderate))
                {
                    var moderate = createPipeLine.ExtendedData[Constants.ExtendedDataKeys.Moderate] as bool?;

                    if (moderate == true)
                    {
                        // Tell the user the topic is awaiting moderation
                        TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
                        {
                            Message = LocalizationService.GetResourceString("Moderate.AwaitingModeration"),
                            MessageType = GenericMessages.info
                        };

                        return RedirectToAction("Index", "VerAnuncios");
                    }
                }
            }

            return View();
        }

        private void CarregarCamposTopicViewModel(Topic topic, NovoAnuncioViewModel model)
        {
            topic.Name = model.TituloAnuncio;
            topic.TipoAnuncio = model.TipoAnuncio;
            topic.IsAnuncio = true;
            topic.IsMecanico = model.TipoCategoria.Equals("Mecânico") ? true : (bool?)null;
            topic.IsInstrutor = model.TipoCategoria.Equals("Instrutor") ? true : (bool?)null;
            topic.IsOperador = model.TipoCategoria.Equals("Operador") ? true : (bool?)null;
            topic.IsCategoryExchange = model.TipoCategoria.Equals("Troca") ? true : (bool?)null;
            topic.IsCategoryNew = model.TipoCategoria.Equals("Novos") ? true : (bool?)null;
            topic.IsCategoryUsed = model.TipoCategoria.Equals("Usados") ? true : (bool?)null;
            topic.Marca = model.Marca;
            topic.Modelo = model.Modelo;
            topic.Price = model.Valor;

            // Usuário
            topic.User.Id = model.Usuario.IdUsuarioLogado;
            topic.TelefoneUsuario = model.Usuario.Telefone;
            topic.TelefoneWhatsApp = model.Usuario.TelefoneZAP;
            topic.CidadeUsuario = model.Usuario.CidadeAdicional;
            topic.EstadoUsuario = model.Usuario.EstadoAdicional;
        }

        private void PreencherUsuario(MembershipUser usuario)
        {
            var dadosUsuario = new DadosUsuario();

            dadosUsuario.IdUsuarioLogado = usuario.Id;
            dadosUsuario.Cidade = usuario.Cidade == null ? "" : usuario.Cidade;
            dadosUsuario.Estado = usuario.Estado == null ? "" : usuario.Estado;
            dadosUsuario.Email = usuario.Email;
            dadosUsuario.NomeUsuario = usuario.UserName;

            ViewBag.Usuario = dadosUsuario;
        }

        private List<Category> AllowedCreateCategories(MembershipRole loggedOnUsersRole)
        {
            var allowedAccessCategories = _categoryService.GetAllowedCategories(loggedOnUsersRole);

            var allowedCreateTopicCategories =
                _categoryService.GetAllowedCategories(loggedOnUsersRole,
                    ForumConfiguration.Instance.PermissionCreateTopics);

            var allowedCreateTopicCategoryIds = allowedCreateTopicCategories.Select(x => x.Id);

            if (allowedAccessCategories.Any())
            {
                allowedAccessCategories.RemoveAll(x => allowedCreateTopicCategoryIds.Contains(x.Id));
                allowedAccessCategories.RemoveAll(x =>
                    loggedOnUsersRole.RoleName != Constants.AdminRoleName && x.IsLocked);
            }

            return allowedAccessCategories;
        }

        private static CheckCreateTopicPermissions GetCheckCreateTopicPermissions(PermissionSet permissionSet)
        {
            var model = new CheckCreateTopicPermissions();

            if (permissionSet[ForumConfiguration.Instance.PermissionCreateStickyTopics].IsTicked)
                model.CanStickyTopic = true;

            if (permissionSet[ForumConfiguration.Instance.PermissionLockTopics].IsTicked)
                model.CanLockTopic = true;

            if (permissionSet[ForumConfiguration.Instance.PermissionAttachFiles].IsTicked)
                model.CanUploadFiles = true;

            if (permissionSet[ForumConfiguration.Instance.PermissionCreatePolls].IsTicked)
                model.CanCreatePolls = true;

            if (permissionSet[ForumConfiguration.Instance.PermissionInsertEditorImages].IsTicked)
                model.CanInsertImages = true;

            if (permissionSet[ForumConfiguration.Instance.PermissionCreateTags].IsTicked)
                model.CanCreateTags = true;

            return model;
        }
    }
}