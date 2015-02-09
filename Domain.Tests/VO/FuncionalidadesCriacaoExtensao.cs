using System;
using Domain.VO;
using Xbehave;
using Xunit;

namespace Domain.Tests.VO {
    public class FuncionalidadesCriacaoExtensao {
        [Scenario]
        public void Gera_erro_se_numero_extensao_tiver_letras(Action criacao, Exception excecaoEsperada) {
            "Dada uma operação de criação de uma extensão"
                .Given(() => criacao = () => Contacto.CriaExtensao("123d"));

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
        public void Gera_erro_se_numero_extensao_nao_tiver_numero_esperado_digitos(Action criacao, Exception excecaoEsperada) {
            "Dada uma operação de criação de uma extensão"
                .Given(() => criacao = () => Contacto.CriaExtensao("123"));

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
        public void Cria_telefone_se_numero_tiver_4_digitos(Action criacao, Exception excecaoEsperada) {
            "Dada uma operação de criação de uma extensão"
                .Given(() => criacao = () => Contacto.CriaExtensao("1234"));

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