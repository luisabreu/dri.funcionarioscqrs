using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Domain.Servicos {
    [ContractClass(typeof(ContratoServicoDuplicacaoNif))]
    public interface IServicoDuplicacaoNif {
        Task<bool> NifDuplicado(string nif, Guid id);
    }

    [ContractClassFor(typeof(IServicoDuplicacaoNif))]
    abstract class ContratoServicoDuplicacaoNif : IServicoDuplicacaoNif {
        public Task<bool> NifDuplicado(string nif, Guid id) {
            Contract.Requires(!string.IsNullOrEmpty(nif));
            return default (Task<bool>);
        }
    }
}