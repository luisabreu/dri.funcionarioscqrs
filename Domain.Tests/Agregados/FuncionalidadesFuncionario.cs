using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Agregados;
using Domain.Mensagens;
using Domain.Mensagens.Comandos;
using Domain.Mensagens.Eventos;
using FluentAssertions;
using Xbehave;

namespace Domain.Tests.Agregados {
    public class FuncionalidadesFuncionario {
        [Scenario]
        public void Tratamento_novo_funcionario(CriaFuncionario comando, Funcionario funcionario) {
            "Dado um comando de criação"
                .Given(() => comando = new CriaFuncionario(Guid.NewGuid(), "Luis", "123456789", 1));

            "Quando criamos um funcionário"
                .When(() => funcionario = new Funcionario(comando));

            "Então devemos ter o evento FuncionarioCriado nos eventos a persistir"
                .Then( () => funcionario.ObtemEventosNaoPersistidos().First()
                                        .Should().BeAssignableTo<FuncionarioCriado>());
        }

        [Scenario]
        public void Tratamento_modificacao_dados_gerais(Funcionario funcionario, ModificaDadosGeraisFuncionario comando, IEnumerable<IEvento> esperados ) {
            var id = Guid.NewGuid();
            "Dado um funcionário existente"
                .Given(() => {
                           funcionario = new Funcionario();
                           funcionario.CarregaDeHistorico(new[] {new FuncionarioCriado(id, "Luis", "123456789", 1)});
                       });

            "E um comando de modificação de dados gerais"
                .And(() => comando = new ModificaDadosGeraisFuncionario(id, 1, "LM", "123456789", 2));

            "Quando aplicamos o comando"
                .When(() => funcionario.ModificaDadosGerais(comando));

            "Então devemos obter um evento DadosGeraisFuncionarioModificados"
                .Then(() => funcionario.ObtemEventosNaoPersistidos().First().Should().BeAssignableTo<DadosGeraisFuncionarioModificados>());

            "E os campos devem ter sido inicializados corretamente"
                .And(() => {
                         var evt = funcionario.ObtemEventosNaoPersistidos().First() as DadosGeraisFuncionarioModificados;
                         evt.Id.Should().Be(id);
                         evt.IdTipoFuncionario.Should().Be(comando.IdTipoFuncionario);
                         evt.Nif.Should().Be(comando.Nif);
                         evt.Nome.Should().Be(comando.Nome);
                     });

        }
        
        [Scenario]
        public void Tratamento_modificacao_dados_gerais_com_id_incorreto(Funcionario funcionario, ModificaDadosGeraisFuncionario comando, Exception excecaoEsperada) {
            var id = Guid.NewGuid();
            "Dado um funcionário existente"
                .Given(() => {
                           funcionario = new Funcionario();
                           funcionario.CarregaDeHistorico(new[] {new FuncionarioCriado(id, "Luis", "123456789", 1)});
                       });

            "E um comando de modificação de dados gerais"
                .And(() => comando = new ModificaDadosGeraisFuncionario(Guid.NewGuid(), 1, "LM", "123456789", 2));

            "Quando aplicamos o comando"
                .When(() => {
                          try {
                              funcionario.ModificaDadosGerais(comando);
                          }
                          catch (Exception ex) {
                              excecaoEsperada = ex;
                          }
                      });

            "Então devemos obter uma excecao"
                .Then(() => excecaoEsperada.Should().BeAssignableTo<InvalidOperationException>());

        }
    }
}