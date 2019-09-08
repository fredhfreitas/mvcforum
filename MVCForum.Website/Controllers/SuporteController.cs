using CommonServiceLocator;
using MvcForum.Core.Interfaces;
using MvcForum.Core.Interfaces.Services;
using MvcForum.Core.Models;
using MvcForum.Core.Models.Entities;
using MvcForum.Web.Controllers;
using MvcForum.Web.ViewModels;
using System;
using System.Configuration;
using System.Text;
using System.Web.Mvc;
using MvcForum.Core.Constants;
using Constants = MvcForum.Core.Constants.Constants;
using MvcForum.Core.ExtensionMethods;

namespace MvcForum.Web.Views.Suporte
{
    public partial class SuporteController : BaseController
    {
        private readonly IActivityService _activityService;
        private readonly ICategoryService _categoryService;
        private readonly ITopicService _topicService;
        private readonly IEmailService _emailService;
        private readonly Settings _settings;

        public SuporteController(ILoggingService loggingService, IActivityService activityService,
            IMembershipService membershipService, ITopicService topicService, ILocalizationService localizationService,
            IRoleService roleService, ISettingsService settingsService, ICategoryService categoryService,
            ICacheService cacheService, IMvcForumContext context, IEmailService emailService)
            : base(loggingService, membershipService, localizationService, roleService,
                settingsService, cacheService, context)
        {
            _topicService = topicService;
            _categoryService = categoryService;
            _activityService = activityService;
            _emailService = emailService;
            _settings = SettingsService.GetSettings();
        }
        // GET: Suporte
        public ActionResult Manuais()
        {
            return View();
        }

        public ActionResult Contato()
        {
            var loggedOnReadOnlyUser = User.GetMembershipUser(MembershipService);
            var loggedOnUsersRole = loggedOnReadOnlyUser.GetRole(RoleService);
            
            if (loggedOnReadOnlyUser == null)
            {
                return Content("Você precisa estar logado para executar essa ação.");
            }

            return View();
        }

        [HttpPost]
        public ActionResult ContactForm()
        {
            if (ModelState.IsValid)
            {
                var settings = SettingsService.GetSettings();
                var sb = new StringBuilder();
                sb.Append($"<p>{string.Concat("Nome: ", Request.Form["Name"].ToString())}</p>");
                sb.Append($"<p>{string.Concat("Email: ", Request.Form["Email"].ToString())}</p>");
                sb.Append($"<p>{string.Concat("Telefone: ", Request.Form["Telefone"].ToString())}</p>");
                sb.Append($"<p>{string.Concat("Assunto: ", Request.Form["Assunto"].ToString())}</p>");
                sb.Append($"<p>{string.Concat("Mensagem: ", Request.Form["Message"].ToString())}</p>");
                var email = new Email
                {
                    EmailTo = ConfigurationSettings.AppSettings["EmailContato"].ToString(), //settings.AdminEmailAddress,
                    NameTo = "Olá Administrador, temos uma nova mensagem",
                    Subject = "[Case da Retro] [Fale com a CASE] "
                };
                email.Body = _emailService.EmailTemplate(email.NameTo, sb.ToString(), "header-nova-mensagem.jpg");
                _emailService.SendMail(email);

                var message = new GenericMessageViewModel
                {
                    Message = "Enviado com sucesso.",
                    MessageType = GenericMessages.success
                };

                try
                {
                    Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Context.RollBack();
                    LoggingService.Error(ex);
                    message.Message = "Erro ao enviar email.";
                    message.MessageType = GenericMessages.danger;
                }
                TempData[Constants.MessageViewBagName] = message;

                return View("Index");
            }
            else
            {
                return View("Index");
            }
        }
    }
}