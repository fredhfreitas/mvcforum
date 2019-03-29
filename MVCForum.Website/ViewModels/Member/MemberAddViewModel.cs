namespace MvcForum.Web.ViewModels.Member
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Application;
    using System.Web;
    using Core.Models.Entities;
    using Core.Models.Enums;

    public class MemberAddViewModel
    {
        

        [Required(ErrorMessage = "O campo Nome é obrigatório")]
        [ForumMvcResourceDisplayName("Members.Label.Username")]
        [StringLength(150, MinimumLength = 4)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "O campo Endereço de e-mail é obrigatório")]
        [EmailAddress]
        [ForumMvcResourceDisplayName("Members.Label.EmailAddress")]        
        public string Email { get; set; }


        [Required(ErrorMessage = "O campo Senha é obrigatório")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [ForumMvcResourceDisplayName("Members.Label.Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        [ForumMvcResourceDisplayName("Members.Label.ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "O campo Telefone é obrigatório")]
        [StringLength(150, MinimumLength = 4)]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "O campo Cidade é obrigatório")]
        [StringLength(150, MinimumLength = 2)]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "O campo Estado é obrigatório")]
        [StringLength(150, MinimumLength = 2)]
        public string Estado { get; set; }
        public bool DisableFileUploads { get; set; }

        public string Avatar { get; set; }

        [Required(ErrorMessage = "Por favor selecione o arquivo.")]
        public HttpPostedFileBase[] Files { get; set; }

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

        [Required]
        public int MinPasswordLength { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.UserIsApproved")]
        public bool IsApproved { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.Comment")]
        public string Comment { get; set; }

        [ForumMvcResourceDisplayName("Members.Label.Roles")]
        public string[] Roles { get; set; }

        public IList<MembershipRole> AllRoles { get; set; }
        public string SpamAnswer { get; set; }
        public string ReturnUrl { get; set; }
        public string SocialProfileImageUrl { get; set; }
        public string UserAccessToken { get; set; }
        public LoginType LoginType { get; set; }
    }
}