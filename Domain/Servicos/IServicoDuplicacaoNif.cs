using System;
using System.Diagnostics.Contracts;

namespace Domain.Servicos {
    [ContractClass(typeof(ContratoServicoDuplicacaoNif))]
    public interface IServicoDuplicacaoNif {
        bool NifDuplicado(string nif, Guid id);
    }

    [ContractClassFor(typeof(IServicoDuplicacaoNif))]
    abstract class ContratoServicoDuplicacaoNif : IServicoDuplicacaoNif {
        public bool NifDuplicado(string nif, Guid id) {
            Contract.Requires(!string.IsNullOrEmpty(nif));
            return default (bool);
        }
    }
}