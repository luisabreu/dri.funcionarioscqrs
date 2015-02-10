using System;
using Domain.Mensagens.Comandos;
using FluentAssertions;
using Xbehave;

namespace Domain.Tests.Messages.Comandos {
    public class FuncionalidadesModificaDadosGeraisFuncionario {
        [Scenario]
        public void Criada_do_comando_com_sucesso(Guid id,
            int versao,
            string nome,
            string nif,
            int tipoFuncionario,
            ModificaDadosGeraisFuncionario comando) {
            "Quando recebemos um nome"
                .Given(() => nome = "Luis");

            "Um nif válido"
                .And(() => nif = "123456789");

            "Um tipo de funcionario"
                .And(() => tipoFuncionario = 1);

            "Um id"
                .And(() => id = Guid.NewGuid());

            "E uma versão"
                .And(() => versao = 1);

            "Quando criamos um comando"
                .When(() => comando = new ModificaDadosGeraisFuncionario(id, versao, nome, nif, tipoFuncionario));

            "Então devemos ter o campo nome devidamente inicializado"
                .Then(() => comando.Nome.Should().Be(nome));

            "E o campo nif correto"
                .And(() => comando.Nif.Should().Be(nif));

            "E o campo tipo funcionario correto"
                .And(() => comando.IdTipoFuncionario.Should().Be(tipoFuncionario));

            "E o campo Id correto"
                .And(() => comando.Id.Should().Be(comando.Id));

            "E o campo Versao correto"
                .And(() => comando.Versao.Should().Be(comando.Versao));
        }

        [Scenario]
        public void Criacao_sem_sucesso_comando(Guid id,
            int versao,
            string nome,
            string nif,
            int tipoFuncionario,
            Exception excecaoEsperada) {
            "Quando recebemos um nome"
                .Given(() => nome = "Luis");

            "Um nif inválido"
                .And(() => nif = "333333333");

            "Um tipo de funcionario"
                .And(() => tipoFuncionario = 1);

            "Um id"
                .And(() => id = Guid.NewGuid());

            "E uma versão"
                .And(() => versao = 1);

            "Quando criamos um comando"
                .When(() => {
                    try {
                        new ModificaDadosGeraisFuncionario(id, versao, nome, nif, tipoFuncionario);
                    }
                    catch (ArgumentException ex) {
                        excecaoEsperada = ex;
                    }
                });

            "Então devemos obter uma exceção"
                .Then(() => excecaoEsperada.Should().NotBeNull());
        }
    }
}