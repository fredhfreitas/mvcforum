namespace MvcForum.Web.ViewModels.Member
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Mvc;
    using Application;
    using Core.Constants;

    public class MemberFrontEndEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [ForumMvcResourceDisplayName("Members.Label.Username")]
        [StringLength(150, MinimumLength = 4)]
        public string UserName { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.EmailAddress")]
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.Signature")]
        [StringLength(1000)]
        [UIHint(Constants.EditorType)]
        [AllowHtml]
        public string Signature { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.Age")]
        [Range(0, int.MaxValue)]
        public int? Age { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.Location")]
        [StringLength(100)]
        public string Location { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.Website")]
        [Url]
        [StringLength(100)]
        public string Website { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.Twitter")]
        [Url]
        [StringLength(60)]
        public string Twitter { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.UploadNewAvatar")]
        public HttpPostedFileBase[] Files { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.Facebook")]
        [Url]
        [StringLength(60)]
        public string Facebook { get; set; }

        public string Avatar { get; set; }
        public bool DisableFileUploads { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.DisableEmailNotifications")]
        public bool DisableEmailNotifications { get; set; }


        public int AmountOfPoints { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string Cidade { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 2)]
        public string Estado { get; set; }


        public string FuncaoEmpresario { get; set; }
        public string FuncaoOperador { get; set; }
        public string FuncaoMecanico { get; set; }
        public string FuncaoOutro { get; set; }


        public string Marca1 { get; set; }
        public string Modelo1 { get; set; }
        public string AnoFabricacao1 { get; set; }
        public string Empresa1 { get; set; }

        public string Marca2 { get; set; }
        public string Modelo2 { get; set; }
        public string AnoFabricacao2 { get; set; }
        public string Empresa2 { get; set; }

        public string Marca3 { get; set; }
        public string Modelo3 { get; set; }
        public string AnoFabricacao3 { get; set; }
        public string Empresa3 { get; set; }

        public string Marca4 { get; set; }
        public string Modelo4 { get; set; }
        public string AnoFabricacao4 { get; set; }
        public string Empresa4 { get; set; }

        public string Marca5 { get; set; }
        public string Modelo5 { get; set; }
        public string AnoFabricacao5 { get; set; }
        public string Empresa5 { get; set; }
    }
}