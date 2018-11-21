using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcForum.Web.ViewModels.Contact
{
    public class ContactFormModel
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required(ErrorMessage = "O Email é obrigatório.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "A Mensagem é obrigatória.")]
        public string Message { get; set; }
    }
}