namespace MvcForum.Web.ViewModels.Topic
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web;
    using System.Web.Mvc;
    using Application;
    using Core.Constants;
    using Core.Models.Entities;

    public class CreateEditTopicEventViewModel
    {
        
        [Required]
        [StringLength(100)]
        [ForumMvcResourceDisplayName("Topic.Label.TopicTitle")]
        public string Name { get; set; }

        [UIHint(Constants.EditorType)]
        [AllowHtml]
        [StringLength(60000)]
        public string Content { get; set; }
                
        // Edit Properties
        [HiddenInput]
        public Guid Id { get; set; }

        [HiddenInput]
        public Guid TopicId { get; set; }

        public bool IsTopicStarter { get; set; }
        

        [ForumMvcResourceDisplayName("Topic.Label.UploadFiles")]
        public HttpPostedFileBase[] Files { get; set; }

        public bool IsPostEdit { get; set; }

        [Required(ErrorMessage = "Data Início é obrigatório")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}",  ApplyFormatInEditMode = true)]
        [ForumMvcResourceDisplayName("Data Início")]
        public DateTime DataEventoInicio { get; set; }
        [Required(ErrorMessage = "Data Fim é obrigatório")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [ForumMvcResourceDisplayName("Data Fim")]
        public DateTime DataEventoFim { get; set; }
        [Required(ErrorMessage = "Hora Início é obrigatório")]
        [ForumMvcResourceDisplayName("Hora Início")]
        public TimeSpan HoraInicio { get; set; }
        [Required(ErrorMessage = "Hora Fim é obrigatório")]
        [ForumMvcResourceDisplayName("Hora Fim")]
        public TimeSpan HoraFim { get; set; }
        [Required(ErrorMessage = "Local é obrigatório")]
        [ForumMvcResourceDisplayName("Local")]
        public string LocalEvento { get; set; }
        //[Required(ErrorMessage = "Ingressos é obrigatório")]
        [ForumMvcResourceDisplayName("Ingressos")]
        public string LinkExternoEvento { get; set; }
        [Required(ErrorMessage = "Cidade é obrigatório")]
        [ForumMvcResourceDisplayName("Cidade")]
        public string CidadeEvento { get; set; }
        [Required(ErrorMessage = "Estado é obrigatório")]
        [ForumMvcResourceDisplayName("Estado")]
        public string EstadoEvento { get; set; }
    }
}