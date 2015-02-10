using System;
using System.Collections.Generic;
using Domain.Comandos;
using Domain.VO;
using FluentAssertions;
using Xbehave;

namespace Domain.Tests.Comandos {
    public class FuncionalidadesCriaFuncionario {
        [Scenario]
        public void Cenario_criacao(string nome, string nif, int tipoFuncionario, IEnumerable<Contacto> contactos, CriaFuncionario comando) {
            "Quando recebemos um nome"
                .Given(() => nome = "Luis");

            "Um nif válido"
                .And(() => nif = "123456789");

            "Um tipo de funcionario"
                .And(() => tipoFuncionario = 1);

            "E uma coleção de contactos"
                .And(() => contactos = new[] {Contacto.CriaExtensao("1234")});

            "Quando criamos um comando"
                .When(() => comando = new CriaFuncionario(Guid.NewGuid(), nome, nif, tipoFuncionario, contactos));

            "Então devemos ter o campo nome devidamente inicializado"
                .Then(() => comando.Nome.Should().Be(nome));

            "E o campo nif correto"
                .And(() => comando.Nif.Should().Be(nif));

            "E o campo tipo funcionario correto"
                .And(() => comando.IdTipoFuncionario.Should().Be(tipoFuncionario));

            "E o campo contactos devidamente inicializado"
                .And(() => comando.Contactos.Should().BeEquivalentTo(contactos));
        }
        
        [Scenario]
        public void Cenario_criacao_com_nif_invalido(string nome, string nif, int tipoFuncionario, IEnumerable<Contacto> contactos, Exception excecaoEsperada) {
            "Quando recebemos um nome"
                .Given(() => nome = "Luis");

            "Um nif inválido"
                .And(() => nif = "333333333");

            "Um tipo de funcionario"
                .And(() => tipoFuncionario = 1);

            "E uma coleção de contactos"
                .And(() => contactos = new[] {Contacto.CriaExtensao("1234")});

            "Quando criamos um comando"
            .When(() => {
                try {
                    new CriaFuncionario(Guid.NewGuid(), nome, nif, tipoFuncionario, contactos);
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