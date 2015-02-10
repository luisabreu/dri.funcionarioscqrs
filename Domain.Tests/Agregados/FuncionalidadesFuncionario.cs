using System;
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
    }
}