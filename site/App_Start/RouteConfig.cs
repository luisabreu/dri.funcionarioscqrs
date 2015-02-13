using System.Web.Mvc;
using System.Web.Routing;

namespace site {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default",
                "{action}/{nifOuNome}",
                new {controller = "Funcionarios", action = "Index", nifOuNome = UrlParameter.Optional}
                );
        }
    }
}