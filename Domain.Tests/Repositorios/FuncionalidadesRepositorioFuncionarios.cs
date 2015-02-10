using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Agregados;
using Domain.Mensagens;
using Domain.Mensagens.Comandos;
using Domain.Repositorios;
using FluentAssertions;
using Moq;
using Xbehave;

namespace Domain.Tests.Repositorios {
    public class FuncionalidadesRepositorioFuncionarios {
        private readonly AutoMockContainer _container = new AutoMockContainer(new MockRepository(MockBehavior.Strict));

        [Scenario]
        public void Cenario_carregamento_de_funcionario(RepositorioFuncionarios repositorio, Guid id, Funcionario carregado) {
            IEnumerable<IEvento> eventos = new[] {new FuncionarioCriado(Guid.NewGuid(), "Luis", "123456789", 1)};

            "Dado um repositorio"
                .Given(() => repositorio = _container.Create<RepositorioFuncionarios>());

            "E um id válido"
                .And(() => id = Guid.NewGuid());

            "E alguns mocks"
                .And(() => {
                         var streamId = id.ToString();
                         _container.GetMock<IDepositoEventos>()
                             .Setup(d => d.ObtemEventosParaAgregado(streamId, 0))
                             .Returns(Task.FromResult(eventos));
                     });

            "Quando tentamos recuperar o funcionário do repositório"
                .When(() => carregado = repositorio.Obtem(id).Result);

            "Então devemos obter um funcionario"
                .Then(() => carregado.Should().NotBeNull());

            "E mocks devem ter sido usados"
                .And(() => _container.GetMock<IDepositoEventos>().VerifyAll());
        }
        
        [Scenario]
        public void Cenario_gravacao_eventos(RepositorioFuncionarios repositorio, Funcionario funcionario) {
            "Dado um repositorio"
                .Given(() => repositorio = _container.Create<RepositorioFuncionarios>());

            "E um funcionário"
                .And(() => funcionario = new Funcionario(new CriaFuncionario(Guid.NewGuid(), "luis", "123456789", 1)));

            "E alguns mocks"
                .And(() => {
                         var streamId = funcionario.Id.ToString();
                         _container.GetMock<IDepositoEventos>()
                             .Setup(d => d.GravaEventos(streamId, typeof(Funcionario).Name.ToLower(), funcionario.ObtemEventosNaoPersistidos(), 0))
                             .Returns(Task.FromResult(0));
                     });

            "Quando tentamos gravar um funcionário no repositório"
                .When(() => repositorio.Grava(funcionario, 0 ) );

            "Então não devemos obter erros e mocks devem ter sido usados"
                .Then(() => _container.GetMock<IDepositoEventos>().VerifyAll());
        }

        private class EventoTeste : IEvento {
            public Guid Id { get; set; }
            public int Versao { get; set; }
        }
    }
}