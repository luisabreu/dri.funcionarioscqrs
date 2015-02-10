using System;
using System.Text;
using Domain.Mensagens;
using Domain.Repositorios;
using Newtonsoft.Json;
using Xbehave;
using Xunit;

namespace Domain.Tests.Repositorios {
    public class FuncionalidadesServicoVerificacaoNif {
        [Scenario]
        public void Deve_seriar_evento(SeriadorEventos seriador, EventoTeste evento) {
            "Dado um seriador"
                .Given(() => seriador = new SeriadorEventos());

            "E um evento predefinido"
                .And(() => evento = new EventoTeste(Guid.NewGuid(), 1));

            byte[] eventoSeriado = null;
            "Quando efetuarmos a seriação"
                .When(() => eventoSeriado = seriador.SeriaEvento(evento));

            "Então devemos obter o array de bytes esperado"
                .Then(() => {
                          Assert.NotNull(eventoSeriado);
                          var recuperado = (EventoTeste) JsonConvert.DeserializeObject(
                              Encoding.UTF8.GetString((eventoSeriado)),
                              new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects});
                          Assert.Equal(evento.Id, recuperado.Id);
                      });
        }

        public class EventoTeste : IEvento {
            public EventoTeste(Guid id, int versao) {
                Id = id;
                Versao = versao;
            }

            public Guid Id { get; private set; }
            public int Versao { get; set; }
        }
    }
}