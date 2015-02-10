using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Domain.Mensagens;

namespace Domain.Repositorios {
    [ContractClass(typeof (ContratoDepositoEventos))]
    public interface IDepositoEventos {
        Task GravaEventos(string idStream, string tipo, IEnumerable<IEvento> eventosGravar, int versaoEsperada);
        Task<IEnumerable<IEvento>> ObtemEventosParaAgregado(string idStream, int versaoEsperada = 0);
    }

    [ContractClassFor(typeof (IDepositoEventos))]
    internal abstract class ContratoDepositoEventos : IDepositoEventos {
        public Task GravaEventos(string idStream, string tipo, IEnumerable<IEvento> eventosGravar, int versaoEsperada) {
            Contract.Requires(!string.IsNullOrEmpty(idStream));
            Contract.Requires(eventosGravar != null);
            return default(Task);
        }

        public Task<IEnumerable<IEvento>> ObtemEventosParaAgregado(string idStream, int versaoEsperada = 0) {
            Contract.Requires(!string.IsNullOrEmpty(idStream));
            return default(Task<IEnumerable<IEvento>>);
        }
    }
}