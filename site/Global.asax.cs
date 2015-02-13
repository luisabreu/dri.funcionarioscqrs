using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Domain.Handlers.Eventos;
using EventStore.ClientAPI;
using site.App_Start;

namespace site {
    public class MvcApplication : HttpApplication {

        private static GestorSubscricoes _gestorSubscricoes;
        private static IEventStoreConnection _ligacaoGlobal;

        protected void Application_Start() {
            var container = AutofacConfig.RegisterForMvc();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            _ligacaoGlobal = container.Resolve<IEventStoreConnection>();
            _ligacaoGlobal.ConnectAsync();

            _gestorSubscricoes = container.Resolve<GestorSubscricoes>();
            _gestorSubscricoes.Subscreve();
        }

        protected void Application_End() {
            _gestorSubscricoes.CancelaSubscricao();
            _ligacaoGlobal.Close();
        }
    }
}