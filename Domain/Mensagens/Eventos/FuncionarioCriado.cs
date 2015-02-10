using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using Domain.VO;

namespace Domain.Mensagens.Eventos {
    public class FuncionarioCriado : IEvento {
        public readonly IEnumerable<Contacto> Contactos;
        public readonly int IdTipoFuncionario;
        public readonly string Nif;
        public readonly string Nome;

        public FuncionarioCriado(Guid idFuncionario,
            string nome,
            string nif,
            int idTipoFuncionario,
            IEnumerable<Contacto> contactos = null) {
            Contract.Requires(idFuncionario != null);
            Contract.Requires(!string.IsNullOrEmpty(nome));
            Contract.Requires(!string.IsNullOrEmpty(nif));
            Contract.Ensures(!string.IsNullOrEmpty(Nome));
            Contract.Ensures(Contactos != null);
            Contract.Ensures(!string.IsNullOrEmpty(Nif));
            Id = idFuncionario;
            Nome = nome;
            Nif = nif;
            IdTipoFuncionario = idTipoFuncionario;
            Contactos = contactos ?? Enumerable.Empty<Contacto>();
        }

        public Guid Id { get; private set; }
        public int Versao { get; set; }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(Id != Guid.Empty);
            Contract.Invariant(!string.IsNullOrEmpty(Nome));
            Contract.Invariant(!string.IsNullOrEmpty(Nif));
            Contract.Invariant(IdTipoFuncionario > 0);
            Contract.Invariant(Contactos != null);
        }
    }
}