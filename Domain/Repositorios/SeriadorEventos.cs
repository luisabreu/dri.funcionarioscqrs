using System.Text;
using Domain.Mensagens;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Domain.Repositorios {
    public class SeriadorEventos : ISeriadorEventos {
        private static readonly JsonSerializerSettings _definicoesJson = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects};

        public byte[] SeriaEvento(IEvento evento) {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evento, Formatting.Indented, _definicoesJson));
        }

        public IEvento DeseriaEvento(ResolvedEvent eventoSeriado) {
            var evento = (IEvento) JsonConvert.DeserializeObject(Encoding.UTF8.GetString((eventoSeriado.Event.Data)), _definicoesJson);
            evento.Versao = eventoSeriado.OriginalEventNumber;
            return evento;
        }
    }
}