using System;

namespace MvcForum.Web.Models
{
    public class DadosUsuario
    {
        public Guid IdUsuarioLogado { get; set; }

        public string NomeUsuario { get; set; }

        public string Telefone { get; set; }

        public string Cidade { get; set; }

        public string Estado { get; set; }

        public string CidadeAdicional { get; set; }

        public string EstadoAdicional { get; set; }

        public string TelefoneZAP { get; set; }

        public string Email { get; set; }
    }
}