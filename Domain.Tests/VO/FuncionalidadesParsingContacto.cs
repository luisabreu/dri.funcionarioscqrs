using Domain.VO;
using FluentAssertions;
using Xbehave;

namespace Domain.Tests.VO {
    public class FuncionalidadesParsingContacto {
        [Scenario]
        public void Deve_gerar_telefone(string telefone, Contacto ct) {
            "Dada uma string com número telefone válido"
                .Given(() => telefone = "123456789");

            "Quando efetuarmos o parsing"
                .When(() => ct = Contacto.Parses(telefone));

            "Devemos obter um contacto válido"
                .Then(() => {
                          ct.Valor.Should().Be(telefone);
                          ct.TipoContacto.Should().Be(TipoContacto.Telefone);
                      });
        }

        [Scenario]
        public void Deve_gerar_extensao(string extensao, Contacto ct) {
            "Dada uma string com número extensao válido"
                .Given(() => extensao = "1234");

            "Quando efetuarmos o parsing"
                .When(() => ct = Contacto.Parses(extensao));

            "Devemos obter um contacto válido"
                .Then(() => {
                          ct.Valor.Should().Be(extensao);
                          ct.TipoContacto.Should().Be(TipoContacto.Extensao);
                      });
        }

        [Scenario]
        public void Deve_gerar_email(string email, Contacto ct) {
            "Dada uma string com email válido"
                .Given(() => email = "teste@gmail.com");

            "Quando efetuarmos o parsing"
                .When(() => ct = Contacto.Parses(email));

            "Devemos obter um contacto válido"
                .Then(() => {
                          ct.Valor.Should().Be(email);
                          ct.TipoContacto.Should().Be(TipoContacto.Email);
                      });
        }

        [Scenario]
        public void Deve_gerar_excecao_contacto_invalido(string invalido, Contacto ct) {
            "Dada uma string com invalido válido"
                .Given(() => invalido = "kkkkk");

            "Quando efetuarmos o parsing"
                .When(() => ct = Contacto.Parses(invalido));

            "Devemos obter um contacto válido"
                .Then(() => ct.Should().BeNull());
        }
    }
}