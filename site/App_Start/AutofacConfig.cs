using System.Configuration;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Domain.Relatorios;
using EventStore.ClientAPI;
using NHibernate;
using GestorTransacoes = site.Models.Relatorios.GestorTransacoes;

namespace site.App_Start {
    public class AutofacConfig {
        private static ISessionFactory _fabricaSessoes = new GestorTransacoes().ObtemFabricaSessoes();

        public static IContainer RegisterForMvc() {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterAssemblyTypes(typeof (Funcionario).Assembly)
                .AsImplementedInterfaces()
                .AsSelf();

            OverrideDependencyRegistration(builder);
            
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            return container;
        }

        private static void OverrideDependencyRegistration(ContainerBuilder builder) {
            builder.Register(c => _fabricaSessoes)
                .As<ISessionFactory>()
                .SingleInstance();
            builder.Register(c => _fabricaSessoes.OpenSession())
                .As<ISession>()
                .InstancePerRequest();

            builder.Register(c => EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, ObtemPorta())))
                .As<IEventStoreConnection>()
                .SingleInstance();
        }

        private static int ObtemPorta() {
            var porta = ConfigurationManager.ConnectionStrings["tcpEventStorePort"].ConnectionString;
            return int.Parse(porta);
        }
    }
}