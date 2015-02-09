using System;
using Domain.VO;
using Xbehave;
using Xunit;

namespace Domain.Tests.VO {
    public class FuncionalidadesCriacaoEmail {
        [Scenario]
        public void Gera_erro_se_email_estiver_errado(Action criacao, Exception excecaoEsperada) {
            "Dada uma operação de criação de um email"
                .Given(() => criacao = () => Contacto.CriaEmail("123ddd123"));

            "Quando a executarmos"
                .When(() => {
                          try {
                              criacao();
                          }
                          catch (Exception e) {
                              excecaoEsperada = e;
                          }
                      });

            "A sua execução deve gerar erro"
                .Then(() => Assert.NotNull(excecaoEsperada));
        }

        [Scenario]
        public void Cria_telefone_se_Email_estiver_certo(Action criacao, Exception excecaoEsperada) {
            "Dada uma operação de criação de um email"
                .Given(() => criacao = () => Contacto.CriaEmail("luisabure@gov-madeira.pt"));

            "Quando a executarmos"
                .When(() => {
                          try {
                              criacao();
                          }
                          catch (Exception e) {
                              excecaoEsperada = e;
                          }
                      });

            "A sua execução deve gerar erro"
                .Then(() => Assert.Null(excecaoEsperada));
        }
    }
}