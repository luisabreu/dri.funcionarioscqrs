using System.Diagnostics.Contracts;
using Domain.Mensagens;
using EventStore.ClientAPI;

namespace Domain.Repositorios {
    [ContractClass(typeof (ContratoSeriadorEventos))]
    public interface ISeriadorEventos {
        byte[] SeriaEvento(IEvento evento);
        IEvento DeseriaEvento(ResolvedEvent eventoSeriado);
    }

    [ContractClassFor(typeof (ISeriadorEventos))]
    internal abstract class ContratoSeriadorEventos : ISeriadorEventos {
        public byte[] SeriaEvento(IEvento evento) {
            Contract.Requires(evento != null);
            return default(byte[]);
        }

        public IEvento DeseriaEvento(ResolvedEvent eventoSeriado) {
            return default(IEvento);
        }
    }
}