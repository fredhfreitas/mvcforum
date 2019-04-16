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
        private readonly IPostService _postService;
        private readonly INotificationService _notificationService;

        public NovoAnuncioController(ILoggingService loggingService, IMembershipService membershipService, IRoleService roleService,
                                     ITopicService topicService, IPostService postService, ICategoryService categoryService, INotificationService notificationService, ILocalizationService localizationService,
                                     ISettingsService settingsService, ICacheService cacheService, IMvcForumContext context)
                                    : base(loggingService, membershipService, localizationService, roleService, settingsService, cacheService, context)
        {
            _topicService = topicService;
            _postService = postService;
            _categoryService = categoryService;
            _notificationService = notificationService;
        }

        public ActionResult Index()
        {
            var usuario = User.GetMembershipUser(MembershipService);

            if (usuario == null)
                return RedirectToAction("Index", "Home");

            PreencherUsuario(usuario);

            return View();
        }

        [Authorize]
        public virtual ActionResult Edit(Guid id)
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
                    string tipoCategoria = string.Empty;
                    if (topic.IsMecanico.HasValue) { tipoCategoria = "Mecânico"; }
                    else if (topic.IsInstrutor.HasValue) { tipoCategoria = "Instrutor"; }
                    else if (topic.IsOperador.HasValue) { tipoCategoria = "Operador"; }
                    else if (topic.IsCategoryExchange.HasValue) { tipoCategoria = "Troca"; }                    
                    else if (topic.IsCategoryNew.HasValue) { tipoCategoria = "Novo"; }
                    else if (topic.IsCategoryUsed.HasValue) { tipoCategoria = "Usado"; }

                    var dadosUsuario = new DadosUsuario();

                    dadosUsuario.IdUsuarioLogado = loggedOnReadOnlyUser.Id;
                    dadosUsuario.Cidade = loggedOnReadOnlyUser.Cidade == null ? "" : loggedOnReadOnlyUser.Cidade;
                    dadosUsuario.Estado = loggedOnReadOnlyUser.Estado == null ? "" : loggedOnReadOnlyUser.Estado;
                    dadosUsuario.Email = loggedOnReadOnlyUser.Email;
                    dadosUsuario.NomeUsuario = loggedOnReadOnlyUser.UserName;

                    var viewModel = new NovoAnuncioViewModel {
                        TituloAnuncio = topic.Name,
                        TipoAnuncio = topic.TipoAnuncio,
                        TipoCategoria = tipoCategoria,
                        Marca = topic.Marca,
                        Modelo = topic.Modelo,
                        Valor = topic.Price.ToString(),
                        ImagemPath = topic.Imagem,
                        // Usuário
                        Usuario = dadosUsuario
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

        //[HttpPost]
        //[ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        //public virtual async Task<ActionResult> EditPostTopic(NovoAnuncioViewModel editPostViewModel)
        //{
        //    // Get the current user and role
        //    var loggedOnUser = User.GetMembershipUser(MembershipService, false);
        //    var loggedOnUsersRole = loggedOnUser.GetRole(RoleService, false);

        //    // Get the category
        //    //var category = _categoryService.Get(editPostViewModel.Category);

        //    // Get all the permissions for this user
        //    var permissions = RoleService.GetPermissions(category, loggedOnUsersRole);

        //    // Now we have the category and permissionSet - Populate the optional permissions 
        //    // This is just in case the viewModel is return back to the view also sort the allowedCategories
        //    // Get the allowed categories for this user
        //    var allowedAccessCategories = _categoryService.GetAllowedCategories(loggedOnUsersRole);
        //    var allowedCreateTopicCategories = _categoryService.GetAllowedCategories(loggedOnUsersRole,
        //        ForumConfiguration.Instance.PermissionCreateTopics);
        //    var allowedCreateTopicCategoryIds = allowedCreateTopicCategories.Select(x => x.Id);

        //    // TODO ??? Is this correct ??
        //    allowedAccessCategories.RemoveAll(x => allowedCreateTopicCategoryIds.Contains(x.Id));

        //    // Set the categories
        //    editPostViewModel.Categories = _categoryService.GetBaseSelectListCategories(allowedAccessCategories);

        //    // Get the users permissions for the topic
        //    editPostViewModel.OptionalPermissions = GetCheckCreateTopicPermissions(permissions);

        //    // See if this is a topic starter or not
        //    editPostViewModel.IsTopicStarter = editPostViewModel.Id == Guid.Empty;

        //    // IS the model valid
        //    if (ModelState.IsValid)
        //    {
        //        // Got to get a lot of things here as we have to check permissions
        //        // Get the post
        //        var originalPost = _postService.Get(editPostViewModel.Id);

        //        // Get the topic
        //        var originalTopic = originalPost.Topic;

        //        // See if the user has actually added some content to the topic
        //        if (string.IsNullOrWhiteSpace(editPostViewModel.Content))
        //        {
        //            ModelState.AddModelError(string.Empty,
        //                LocalizationService.GetResourceString("Errors.GenericMessage"));
        //        }
        //        else
        //        {

        //            bool successful;
        //            bool? moderate = false;
        //            string message;

        //            if (editPostViewModel.IsPostEdit)
        //            {
        //                var editPostPipe = await _postService.Edit(originalPost, editPostViewModel.Files,
        //                    originalPost.IsTopicStarter, string.Empty, editPostViewModel.Content);

        //                successful = editPostPipe.Successful;
        //                message = editPostPipe.ProcessLog.FirstOrDefault();
        //                if (editPostPipe.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.Moderate))
        //                {
        //                    moderate = editPostPipe.ExtendedData[Constants.ExtendedDataKeys.Moderate] as bool?;
        //                }
        //            }
        //            else
        //            {
        //                // Map the new topic (Pass null for new topic)
        //                var topic = editPostViewModel.ToTopic(category, loggedOnUser, originalTopic);

        //                // Run the create pipeline
        //                var editPipeLine = await _topicService.Edit(topic, editPostViewModel.Files,
        //                    editPostViewModel.Tags, editPostViewModel.SubscribeToTopic, editPostViewModel.Content,
        //                    editPostViewModel.Name, editPostViewModel.PollAnswers, editPostViewModel.PollCloseAfterDays);

        //                successful = editPipeLine.Successful;
        //                message = editPipeLine.ProcessLog.FirstOrDefault();
        //                if (editPipeLine.ExtendedData.ContainsKey(Constants.ExtendedDataKeys.Moderate))
        //                {
        //                    moderate = editPipeLine.ExtendedData[Constants.ExtendedDataKeys.Moderate] as bool?;
        //                }
        //            }


        //            // Check if successful
        //            if (successful == false)
        //            {
        //                // Tell the user the topic is awaiting moderation
        //                ModelState.AddModelError(string.Empty, message);
        //                return View(editPostViewModel);
        //            }


        //            if (moderate == true)
        //            {
        //                // Tell the user the topic is awaiting moderation
        //                TempData[Constants.MessageViewBagName] = new GenericMessageViewModel
        //                {
        //                    Message = LocalizationService.GetResourceString("Moderate.AwaitingModeration"),
        //                    MessageType = GenericMessages.info
        //                };

        //                return RedirectToAction("Index", "Home");
        //            }


        //            // Redirect to the newly created topic
        //            return Redirect($"{originalTopic.NiceUrl}?postbadges=true");
        //        }
        //    }

        //    return View(editPostViewModel);
        //}
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
            topic.Price = Convert.ToDecimal(model.Valor);
            if(model.Imagem.First() != null)
                topic.Imagem = string.Format("\\content\\uploads\\{0}\\{1}", model.Usuario.IdUsuarioLogado,model.Imagem.First().FileName);
            else
                topic.Imagem = "\\content\\images\\imagemDefault.jpg";

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