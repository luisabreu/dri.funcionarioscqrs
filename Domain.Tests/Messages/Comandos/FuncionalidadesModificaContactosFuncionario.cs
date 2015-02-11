using System;
using System.Collections.Generic;
using Domain.Mensagens.Comandos;
using Domain.VO;
using FluentAssertions;
using Xbehave;

namespace Domain.Tests.Messages.Comandos {
    public class FuncionalidadesModificaContactosFuncionario {
        [Scenario]
        public void Criada_do_comando_com_sucesso(Guid id,
            int versao,
            IEnumerable<Contacto> contactosAdicionar,
            IEnumerable<Contacto> contactosRemover,
            ModificaContactosFuncionario comando) {
            "Quando recebemos um id"
                .Given(() => id = Guid.NewGuid());

            "Uma versão"
                .And(() => versao = 1);

            "Um conjunto de contactos a adicionar"
                .And(() => contactosAdicionar = new[]{ Contacto.CriaTelefone("123456789")});
            
            "Um conjunto de contactos a remover"
                .And(() => contactosRemover = new[]{ Contacto.CriaTelefone("123456789")});
            
            "Quando criamos um comando"
                .When(() => comando = new ModificaContactosFuncionario(id, versao, contactosAdicionar, contactosRemover));

            "Então devemos ter o campo id devidamente inicializado"
                .Then(() => comando.Id.Should().Be(id));

            "E o campo versao correto"
                .And(() => comando.Versao.Should().Be(versao));

            
            "E o campo contactos adicionar inicializado"
                .And(() => comando.ContactosAdicionar.Should().BeEquivalentTo(contactosAdicionar));
            
            "E o campo contactos remover inicializado"
                .And(() => comando.ContactosRemover.Should().BeEquivalentTo(contactosRemover));

        }
        
        [Scenario]
        public void Criada_do_comando_sem_sucesso(Guid id,
            int versao,
            Exception excecaoEsperada) {
            "Quando recebemos um id"
                .Given(() => id = Guid.NewGuid());

            "Uma versão"
                .And(() => versao = 1);
         
            "Quando criamos um comando"
                .When(() => {
                          try {
                              new ModificaContactosFuncionario(id, versao, null, null);
                          }
                          catch (Exception ex) {
                              excecaoEsperada = ex;
                          }
                      });

            "Então devemos obter uma excecao"
                .Then(() => excecaoEsperada.Should().NotBeNull());

        }
    }
}