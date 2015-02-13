using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Domain.Relatorios;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Log;
using EventStore.ClientAPI.SystemData;
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

            builder.Register(c => EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, ObtemPortaTcp())))
                .As<IEventStoreConnection>()
                .SingleInstance();

            builder.Register(c => new ProjectionsManager(new ConsoleLogger(), new IPEndPoint(IPAddress.Loopback, ObtemPortaHttp()), TimeSpan.FromSeconds(60)))
                .SingleInstance();

            builder.Register(c => new UserCredentials("admin", "changeit"))
                .SingleInstance();
        }

        private static int ObtemPortaTcp() {
            var porta = ConfigurationManager.AppSettings["tcpEventStorePort"];
            return int.Parse(porta);
        }
        
        private static int ObtemPortaHttp() {
            var porta = ConfigurationManager.AppSettings["httpEventStorePort"];
            return int.Parse(porta);
        }
    }
}