using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using Domain.VO;

namespace Domain.Mensagens.Eventos {
    public class ContactosFuncionarioModificados : IEvento {
        public readonly IEnumerable<Contacto> ContactosAdicionar;
        public readonly IEnumerable<Contacto> ContactosRemover;

        public ContactosFuncionarioModificados(Guid id,
            IEnumerable<Contacto> contactosAdicionar = null,
            IEnumerable<Contacto> contactosRemover = null) {
            Contract.Requires(id != Guid.Empty);
            Contract.Ensures(id != Guid.Empty);
            Contract.Ensures(ContactosAdicionar != null);
            Contract.Ensures(ContactosRemover != null);
            Id = id;
            ContactosAdicionar = contactosAdicionar ?? Enumerable.Empty<Contacto>();
            ContactosRemover   = contactosRemover ?? Enumerable.Empty<Contacto>();
        }

        public Guid Id { get; private set; }
        public int Versao { get; set; }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(Id != Guid.Empty);
            Contract.Invariant(ContactosAdicionar != null);
            Contract.Invariant(ContactosRemover != null);
        }
    }
}