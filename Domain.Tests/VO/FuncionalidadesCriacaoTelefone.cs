using System;
using Domain.VO;
using Xbehave;
using Xunit;

namespace Domain.Tests.VO {
    public class FuncionalidadesCriacaoTelefone {
        [Scenario]
        public void Gera_erro_se_numero_telefone_tiver_letras(Action criacao, Exception excecaoEsperada) {
            "Dada uma operação de criação de um telefone"
                .Given(() => criacao = () => Contacto.CriaTelefone("123ddd123"));

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
        public void Gera_erro_se_numero_telefone_nao_tiver_numero_esperado_digitos(Action criacao, Exception excecaoEsperada) {
            "Dada uma operação de criação de um telefone"
                .Given(() => criacao = () => Contacto.CriaTelefone("1231234"));

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
        public void Cria_telefone_se_numero_tiver_9_digitos(Action criacao, Exception excecaoEsperada) {
            "Dada uma operação de criação de um telefone"
                .Given(() => criacao = () => Contacto.CriaTelefone("123123123"));

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