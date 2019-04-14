using System.Web;
using System.Web.Mvc;

namespace MvcForum.Web.Models
{
    public class NovoAnuncioViewModel
    {
        public string TipoServico { get; set; }

        public string TipoCategoria { get; set; }

        [AllowHtml]
        public string Observacao { get; set; }

        public HttpPostedFileBase[] Imagem { get; set; }

        public decimal Valor { get; set; }

        public string Marca { get; set; }

        public string Modelo { get; set; }

        public DadosUsuario Usuario { get; set; }
    }
}