namespace MvcForum.Web.ViewModels.CategoriaAnuncio
{
    public class CategoriaAnuncioViewModel
    {
        /// <summary>
        /// Recebe o nome que virá dentro do H2
        /// </summary>
        public string NomeTitulo { get; set; }

        /// <summary>
        /// URL Título
        /// </summary>
        public string URLTitulo { get; set; }

        /// <summary>
        /// Texto do paragrafo
        /// </summary>
        public string TextoParagrafo { get; set; }

        /// <summary>
        /// Quantidade de Itens no Tópico
        /// </summary>
        public int QuantidadeAnuncio { get; set; }
    }
}