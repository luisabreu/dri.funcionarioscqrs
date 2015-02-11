using System;
using Domain.Handlers;
using Domain.Mensagens.Comandos;
using Domain.Servicos;
using FluentAssertions;
using Moq;
using Xbehave;

namespace Domain.Tests.Handlers {
    public class FuncionalidadesTrataComandosFuncionario {
        private readonly AutoMockContainer _container = new AutoMockContainer(new MockRepository(MockBehavior.Strict));

        [Scenario]
        public void Cenario_nif_duplicado(TrataComandosFuncionario handler, CriaFuncionario comando, Exception excecaoEsperada) {
            "Dado uma handler"
                .Given(() => handler = _container.Create<TrataComandosFuncionario>());

            "E um comando"
                .And(() => comando = new CriaFuncionario(Guid.NewGuid(), "Luis", "123456789", 1));

            "E um verificador de NIF"
                .And(() => _container.GetMock<IServicoDuplicacaoNif>()
                    .Setup(v => v.NifDuplicado(comando.Nif, comando.Id))
                    .Returns(true));

            "Quando tratamos o comando"
                .When(() => {
                          try {
                              handler.Trata(comando);
                          }
                          catch (InvalidOperationException ex) {
                              excecaoEsperada = ex;
                          }
                      });

            "Então devemos obter uma exceção"
                .Then(() => excecaoEsperada.Should().NotBeNull());
        }
        
        [Scenario]
        public void Cenario_tipo_funcionario_inexistente(TrataComandosFuncionario handler, CriaFuncionario comando, Exception excecaoEsperada) {
            "Dado uma handler"
                .Given(() => handler = _container.Create<TrataComandosFuncionario>());

            "E um comando"
                .And(() => comando = new CriaFuncionario(Guid.NewGuid(), "Luis", "123456789", 100));

            "E um verificador de NIF"
                .And(() => _container.GetMock<IServicoDuplicacaoNif>()
                    .Setup(v => v.NifDuplicado(comando.Nif, comando.Id))
                    .Returns(false));
            
            "E um verificador de tipos de funcionário"
                .And(() => _container.GetMock<IServicoVerificacaoTiposFuncionario>()
                    .Setup(v => v.TipoFuncionarioValido(comando.IdTipoFuncionario))
                    .Returns(false));

            "Quando tratamos o comando"
                .When(() => {
                          try {
                              handler.Trata(comando);
                          }
                          catch (InvalidOperationException ex) {
                              excecaoEsperada = ex;
                          }
                      });

            "Então devemos obter uma exceção"
                .Then(() => excecaoEsperada.Should().NotBeNull());
        }
    }
}