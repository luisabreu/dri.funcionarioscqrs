using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Domain.Relatorios;
using NHibernate;

namespace site.App_Start {
    public class AutofacConfig {
        private static ISessionFactory _fabricaSessoes = new site.Models.Relatorios.GestorTransacoes().ObtemFabricaSessoes();

        public static void RegisterForMvc() {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterAssemblyTypes(typeof (Funcionario).Assembly)
                .AsImplementedInterfaces()
                .AsSelf();

            OverrideDependencyRegistration(builder);

            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }

        private static void OverrideDependencyRegistration(ContainerBuilder builder) {
            builder.Register(c => _fabricaSessoes)
                .As<ISessionFactory>()
                .SingleInstance();
            builder.Register(c => _fabricaSessoes.OpenSession())
                .As<ISession>()
                .InstancePerRequest();
        }
    }
}