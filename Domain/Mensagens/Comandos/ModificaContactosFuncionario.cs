using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using Domain.VO;

namespace Domain.Mensagens.Comandos {
    public class ModificaContactosFuncionario : IComando {
        public readonly IEnumerable<Contacto> ContactosAdicionar;
        public readonly IEnumerable<Contacto> ContactosRemover;

        public ModificaContactosFuncionario(Guid id,
            int versao,
            IEnumerable<Contacto> contactosAdicionar = null,
            IEnumerable<Contacto> contactosRemover = null) {
            Contract.Requires(id != Guid.Empty);
            Contract.Requires(versao > 0);
            Contract.Requires(contactosAdicionar != null || contactosRemover != null);
            Contract.Ensures(id != Guid.Empty);
            Contract.Ensures(versao > 0);
            Contract.Ensures(contactosAdicionar != null);
            Contract.Ensures(contactosRemover != null);
            Id = id;
            Versao = versao;
            ContactosAdicionar = contactosAdicionar ?? Enumerable.Empty<Contacto>();
            ContactosRemover = contactosRemover ?? Enumerable.Empty<Contacto>();
        }

        public Guid Id { get; private set; }
        public int Versao { get; private set; }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(Id != Guid.Empty);
            Contract.Invariant(Versao > 0);
            Contract.Invariant(ContactosAdicionar != null);
            Contract.Invariant(ContactosRemover != null);
        }
    }
}