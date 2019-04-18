namespace MvcForum.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using Core;
    using Core.Constants;

    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            RouteTable.Routes.LowercaseUrls = true;
            RouteTable.Routes.AppendTrailingSlash = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});

            routes.MapRoute(
               "TenhoInteresse", // Route name
               "TenhoInteresse",
               new { controller = "Favourite", action = "IndexAnuncio" } // Parameter defaults
           );

            routes.MapRoute(
                "registerDefault", // Route name
                "Registrar",
                new { controller = "Members", action = "Register" } // Parameter defaults
            );

           
            routes.MapRoute(
                "perguntasDefault", // Route name
                "PerguntasFrequentes",
                new { controller = "Home", action = "PerguntasFrequentes" } // Parameter defaults
            );

            routes.MapRoute(
                "manuaisDefault", // Route name
                "Manuais",
                new { controller = "Suporte", action = "Manuais" } // Parameter defaults
            );

            routes.MapRoute(
                "FaleCaseDefault", // Route name
                "FaleComACase",
                new { controller = "Suporte", action = "Contato" } // Parameter defaults
            );

            routes.MapRoute(
                "CategoryDefault", // Route name
                "Categoria",
                new { controller = "Category", action = "Index" } // Parameter defaults
            );

            routes.MapRoute(
                "ContactDefault", // Route name
                "Contato",
                new { controller = "Home", action = "Contact" } // Parameter defaults
            );

            routes.MapRoute(
                "AboutDefault", // Route name
                "Sobre",
                new { controller = "Home", action = "About" } // Parameter defaults
            );

            routes.MapRoute(
                "followingDefault", // Route name
                "Seguindo",
                new { controller = "Home", action = "following" } // Parameter defaults
            );

            routes.MapRoute(
                "noticias2Default", // Route name
                "Noticias",
                new { controller = "Home", action = "TodasNoticias" } // Parameter defaults
            );

            routes.MapRoute(
                "MeusTopicosDefault", // Route name
                "MeusTopicos",
                new { controller = "Home", action = "postedin" } // Parameter defaults
            );

            routes.MapRoute(
                "favouriteDefault", // Route name
                "Favoritos",
                new { controller = "Favourite", action = "index" } // Parameter defaults
            );

            routes.MapRoute(
                "uiniversoRetroDefault", // Route name
                "universoRetro",
                new { controller = "Home", action = "UniversoRetro" } // Parameter defaults
            );

            routes.MapRoute(
               "novoTopicoDefault", // Route name
               "novoTopico",
               new { controller = "Home", action = "novoTopico" } // Parameter defaults
           );

            routes.MapRoute(
               "searchDefault", // Route name
               "busca",
               new { controller = "Search", action = "index" } // Parameter defaults
           );
            // API Attribute Routes
            //routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                "categoryUrls", // Route name
                string.Concat(ForumConfiguration.Instance.CategoryUrlIdentifier, "/{slug}"), // URL with parameters
                new {controller = "Category", action = "Show", slug = UrlParameter.Optional} // Parameter defaults
            );

            routes.MapRoute(
                "memberseditUrls", // Route name
                "profile",                
                new { controller = "Members", action = "Edit", Guid = UrlParameter.Optional } // Parameter defaults
            );

            
            routes.MapRoute(
                "categoryRssUrls", // Route name
                string.Concat(ForumConfiguration.Instance.CategoryUrlIdentifier, "/rss/{slug}"), // URL with parameters
                new
                {
                    controller = "Category",
                    action = "CategoryRss",
                    slug = UrlParameter.Optional
                } // Parameter defaults
            );

            routes.MapRoute(
                "topicUrls", // Route name
                string.Concat(ForumConfiguration.Instance.TopicUrlIdentifier, "/{slug}"), // URL with parameters
                new {controller = "Topic", action = "Show", slug = UrlParameter.Optional} // Parameter defaults
            );

            routes.MapRoute(
               "retornaNews", // Route name
               string.Concat(ForumConfiguration.Instance.Noticia, "/{value}"), // URL with parameters
               new { controller = "Post", action = "GetNoticiaShow", value = UrlParameter.Optional} // Parameter defaults
           );

            routes.MapRoute(
                "memberUrls", // Route name
                string.Concat(ForumConfiguration.Instance.MemberUrlIdentifier, "/{slug}"), // URL with parameters
                new {controller = "Members", action = "GetByName", slug = UrlParameter.Optional} // Parameter defaults
            );

            routes.MapRoute(
                "tagUrls", // Route name
                string.Concat(ForumConfiguration.Instance.TagsUrlIdentifier, "/{tag}"), // URL with parameters
                new {controller = "Topic", action = "TopicsByTag", tag = UrlParameter.Optional} // Parameter defaults
            );

            routes.MapRoute(
                "topicXmlSitemap", // Route name
                "topicxmlsitemap", // URL with parameters
                new {controller = "Home", action = "GoogleSitemap"} // Parameter defaults
            );

            routes.MapRoute(
                "categoryXmlSitemap", // Route name
                "categoryxmlsitemap", // URL with parameters
                new {controller = "Home", action = "GoogleCategorySitemap"} // Parameter defaults
            );

            routes.MapRoute(
                "memberXmlSitemap", // Route name
                "memberxmlsitemap", // URL with parameters
                new {controller = "Home", action = "GoogleMemberSitemap"} // Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
            );

            //.RouteHandler = new SlugRouteHandler()
        }
    }
}