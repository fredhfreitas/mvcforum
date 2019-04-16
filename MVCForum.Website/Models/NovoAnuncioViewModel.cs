using System;
using System.Web;
using System.Web.Mvc;

namespace MvcForum.Web.Models
{
    public class NovoAnuncioViewModel
    {
        public Guid Id { get; set; }
        [HiddenInput]
        public Guid TopicId { get; set; }
        public string TituloAnuncio { get; set; }

        public string TipoAnuncio { get; set; }

        public string TipoCategoria { get; set; }

        [AllowHtml]
        public string Observacao { get; set; }

        public HttpPostedFileBase[] Imagem { get; set; }

        public string ImagemPath { get; set; }

        public string Valor { get; set; }

        public string Marca { get; set; }

        public string Modelo { get; set; }

        public DadosUsuario Usuario { get; set; }

        public Guid CategoriaId
        {
            get
            {
                return new Guid("6B7647AF-93A6-458B-BC66-A9810186C6CC");
            }
        }
    }
}