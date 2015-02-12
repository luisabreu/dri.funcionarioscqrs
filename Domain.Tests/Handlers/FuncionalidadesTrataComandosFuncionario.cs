using System;
using System.Threading.Tasks;
using Domain.Agregados;
using Domain.Handlers;
using Domain.Handlers.Comandos;
using Domain.Mensagens.Comandos;
using Domain.Repositorios;
using Domain.Servicos;
using Domain.VO;
using EventStore.ClientAPI;
using FluentAssertions;
using Moq;
using Xbehave;

namespace Domain.Tests.Handlers {
    public class FuncionalidadesTrataComandosFuncionario {
        private readonly AutoMockContainer _container = new AutoMockContainer(new MockRepository(MockBehavior.Strict));

        [Scenario]
        public async Task Cenario_nif_duplicado(TrataComandosFuncionario handler, CriaFuncionario comando, Exception excecaoEsperada) {
            "Dado uma handler"
                .Given(() => handler = _container.Create<TrataComandosFuncionario>());

            "E um comando"
                .And(() => comando = new CriaFuncionario(Guid.NewGuid(), "Luis", "123456789", 1));

            "E um verificador de NIF"
                .And(() => _container.GetMock<IServicoDuplicacaoNif>()
                    .Setup(v => v.NifDuplicado(comando.Nif, comando.Id))
                    .Returns(Task.FromResult(true)));

            "Quando tratamos o comando"
                .When(async () => {
                                try {
                                    await handler.Trata(comando);
                                }
                                catch (InvalidOperationException ex) {
                                    excecaoEsperada = ex;
                                }
                            });

            "Então devemos obter uma exceção"
                .Then(() => excecaoEsperada.Should().NotBeNull());

            "E o mock foi usado corretamente"
                .And(() => _container.GetMock<IServicoDuplicacaoNif>().VerifyAll());
        }

        [Scenario]
        public async Task Cenario_tipo_funcionario_inexistente(TrataComandosFuncionario handler, CriaFuncionario comando, Exception excecaoEsperada) {
            "Dado uma handler"
                .Given(() => handler = _container.Create<TrataComandosFuncionario>());

            "E um comando"
                .And(() => comando = new CriaFuncionario(Guid.NewGuid(), "Luis", "123456789", 100));

            "E um verificador de NIF"
                .And(() => _container.GetMock<IServicoDuplicacaoNif>()
                    .Setup(v => v.NifDuplicado(comando.Nif, comando.Id))
                    .Returns(Task.FromResult(false)));

            "E um verificador de tipos de funcionário"
                .And(() => _container.GetMock<IServicoVerificacaoTiposFuncionario>()
                    .Setup(v => v.TipoFuncionarioValido(comando.IdTipoFuncionario))
                    .Returns(false));

            "Quando tratamos o comando"
                .When(async () => {
                                try {
                                    await handler.Trata(comando);
                                }
                                catch (InvalidOperationException ex) {
                                    excecaoEsperada = ex;
                                }
                            });

            "Então devemos obter uma exceção"
                .Then(() => excecaoEsperada.Should().NotBeNull());

            "E os mocks foram usados corretamente"
                .And(() => {
                         _container.GetMock<IServicoDuplicacaoNif>().VerifyAll();
                         _container.GetMock<IServicoVerificacaoTiposFuncionario>().VerifyAll();
                     });
        }

        [Scenario]
        public async Task Cenario_gravacao_novo_sucesso(TrataComandosFuncionario handler, CriaFuncionario comando, Exception excecaoEsperada) {
            "Dado uma handler"
                .Given(() => handler = _container.Create<TrataComandosFuncionario>());

            "E um comando"
                .And(() => comando = new CriaFuncionario(Guid.NewGuid(), "Luis", "123456789", 100));

            "E um verificador de NIF"
                .And(() => _container.GetMock<IServicoDuplicacaoNif>()
                    .Setup(v => v.NifDuplicado(comando.Nif, comando.Id))
                    .Returns(Task.FromResult(false)));

            "E um verificador de tipos de funcionário"
                .And(() => _container.GetMock<IServicoVerificacaoTiposFuncionario>()
                    .Setup(v => v.TipoFuncionarioValido(comando.IdTipoFuncionario))
                    .Returns(true));

            "E um repositior previamente preparado"
                .And(() => _container.GetMock<IRepositorioFuncionarios>()
                    .Setup(r => r.Grava(It.IsAny<Funcionario>(), ExpectedVersion.NoStream))
                    .Returns(Task.FromResult(0)));

            "Quando tratamos o comando"
                .When(async () => await handler.Trata(comando));

            "Então todos os mocks foram usados corretamente"
                .Then(() => {
                          _container.GetMock<IServicoDuplicacaoNif>().VerifyAll();
                          _container.GetMock<IServicoVerificacaoTiposFuncionario>().VerifyAll();
                          _container.GetMock<IRepositorioFuncionarios>().VerifyAll();
                      });
        }

        [Scenario]
        public async Task Cenario_alteracao_dados_gerais_sem_sucesso(TrataComandosFuncionario handler, ModificaDadosGeraisFuncionario comando, Exception excecaoEsperada) {
            var id = Guid.NewGuid();

            "Dado uma handler"
                .Given(() => handler = _container.Create<TrataComandosFuncionario>());

            "E um comando"
                .And(() => comando = new ModificaDadosGeraisFuncionario(id, 0, "Luis M", "123456789", 100));

            "E um verificador de NIF"
                .And(() => _container.GetMock<IServicoDuplicacaoNif>()
                    .Setup(v => v.NifDuplicado(comando.Nif, comando.Id))
                    .Returns(Task.FromResult(false)));

            "E um verificador de tipos de funcionário"
                .And(() => _container.GetMock<IServicoVerificacaoTiposFuncionario>()
                    .Setup(v => v.TipoFuncionarioValido(comando.IdTipoFuncionario))
                    .Returns(true));

            "E um repositior previamente preparado"
                .And(() => {
                         _container.GetMock<IRepositorioFuncionarios>()
                             .Setup(r => r.Obtem(id))
                             .Returns(Task.FromResult<Funcionario>(null));
                     });

            "Quando tratamos o comando"
                .When(async () => {
                                try {
                                    await handler.Trata(comando);
                                }
                                catch (InvalidOperationException ex) {
                                    excecaoEsperada = ex;
                                }
                            });

            "Então obtemos uma exceção"
                .Then(() => excecaoEsperada.Should().NotBeNull());

            "E todos os mocks foram usados corretamente"
                .And(() => {
                         _container.GetMock<IServicoDuplicacaoNif>().VerifyAll();
                         _container.GetMock<IServicoVerificacaoTiposFuncionario>().VerifyAll();
                         _container.GetMock<IRepositorioFuncionarios>().VerifyAll();
                     });
        }

        [Scenario]
        public async Task Cenario_alteracao_dados_gerais_com_sucesso(TrataComandosFuncionario handler, ModificaDadosGeraisFuncionario comando) {
            var id = Guid.NewGuid();

            "Dado uma handler"
                .Given(() => handler = _container.Create<TrataComandosFuncionario>());

            "E um comando"
                .And(() => comando = new ModificaDadosGeraisFuncionario(id, 0, "Luis M", "123456789", 100));

            "E um verificador de NIF"
                .And(() => _container.GetMock<IServicoDuplicacaoNif>()
                    .Setup(v => v.NifDuplicado(comando.Nif, comando.Id))
                    .Returns(Task.FromResult(false)));

            "E um verificador de tipos de funcionário"
                .And(() => _container.GetMock<IServicoVerificacaoTiposFuncionario>()
                    .Setup(v => v.TipoFuncionarioValido(comando.IdTipoFuncionario))
                    .Returns(true));

            "E um repositior previamente preparado"
                .And(() => {
                         _container.GetMock<IRepositorioFuncionarios>()
                             .Setup(r => r.Obtem(id))
                             .Returns(Task.FromResult(new Funcionario(new CriaFuncionario(id, "L", "123456789", 1))));

                         _container.GetMock<IRepositorioFuncionarios>()
                             .Setup(r => r.Grava(It.IsAny<Funcionario>(), comando.Versao))
                             .Returns(Task.FromResult(0));
                     });

            "Quando tratamos o comando"
                .When(async () => await handler.Trata(comando));

            "Então todos os mocks foram usados corretamente"
                .Then(() => {
                          _container.GetMock<IServicoDuplicacaoNif>().VerifyAll();
                          _container.GetMock<IServicoVerificacaoTiposFuncionario>().VerifyAll();
                          _container.GetMock<IRepositorioFuncionarios>().VerifyAll();
                      });
        }

        [Scenario]
        public async Task Cenario_alteracao_contactos_sem_sucesso(TrataComandosFuncionario handler, ModificaContactosFuncionario comando, Exception excecaoEsperada) {
            var id = Guid.NewGuid();

            "Dado uma handler"
                .Given(() => handler = _container.Create<TrataComandosFuncionario>());

            "E um comando"
                .And(() => comando = new ModificaContactosFuncionario(id, 0, new[] {Contacto.CriaExtensao("1234")}));


            "E um repositior previamente preparado"
                .And(() => {
                         _container.GetMock<IRepositorioFuncionarios>()
                             .Setup(r => r.Obtem(id))
                             .Returns(Task.FromResult<Funcionario>(null));
                     });

            "Quando tratamos o comando"
                .When(async () => {
                                try {
                                    await handler.Trata(comando);
                                }
                                catch (InvalidOperationException ex) {
                                    excecaoEsperada = ex;
                                }
                            });

            "Então obtemos uma exceção"
                .Then(() => excecaoEsperada.Should().NotBeNull());

            "E todos os mocks foram usados corretamente"
                .And(() => {
                         _container.GetMock<IServicoDuplicacaoNif>().VerifyAll();
                         _container.GetMock<IServicoVerificacaoTiposFuncionario>().VerifyAll();
                         _container.GetMock<IRepositorioFuncionarios>().VerifyAll();
                     });
        }

        [Scenario]
        public async Task Cenario_alteracao_contactos_com_sucesso(TrataComandosFuncionario handler, ModificaContactosFuncionario comando) {
            var id = Guid.NewGuid();

            "Dado uma handler"
                .Given(() => handler = _container.Create<TrataComandosFuncionario>());

            "E um comando"
                .And(() => comando = new ModificaContactosFuncionario(id, 0, new[] {Contacto.CriaExtensao("1234")}));

            "E um repositior previamente preparado"
                .And(() => {
                         _container.GetMock<IRepositorioFuncionarios>()
                             .Setup(r => r.Obtem(id))
                             .Returns(Task.FromResult(new Funcionario(new CriaFuncionario(id, "L", "123456789", 1))));

                         _container.GetMock<IRepositorioFuncionarios>()
                             .Setup(r => r.Grava(It.IsAny<Funcionario>(), comando.Versao))
                             .Returns(Task.FromResult(0));
                     });

            "Quando tratamos o comando"
                .When(async () => await handler.Trata(comando));

            "Então todos os mocks foram usados corretamente"
                .Then(() => {
                          _container.GetMock<IServicoDuplicacaoNif>().VerifyAll();
                          _container.GetMock<IServicoVerificacaoTiposFuncionario>().VerifyAll();
                          _container.GetMock<IRepositorioFuncionarios>().VerifyAll();
                      });
        }
    }
}