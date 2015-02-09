using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using Domain.Mensagens;
using Domain.VO;

namespace Domain.Comandos {
    public class CriaFuncionario : IComando {
        private readonly int IdTipoFuncionario;
        public readonly string Nif;
        public readonly string Nome;
        public readonly IEnumerable<Contacto> Contactos;

        public CriaFuncionario(Guid idAgregado,
            int versao,
            string nome,
            string nif,
            int idTipoFuncionario,
            IEnumerable<Contacto> contactos) {
            Contract.Requires(!string.IsNullOrEmpty(nome));
            Contract.Requires(contactos != null);
            Contract.Requires(!string.IsNullOrEmpty(nif));
            IdAgregado = idAgregado;
            Versao = versao;
            Nome = nome;
            Nif = nif;
            Contactos = contactos ?? Enumerable.Empty<Contacto>();
            IdTipoFuncionario = idTipoFuncionario;
        }


        public Guid IdAgregado { get; private set; }
        public int Versao { get; private set; }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(IdAgregado != Guid.Empty);
            Contract.Invariant(!string.IsNullOrEmpty(Nome));
            Contract.Invariant(!string.IsNullOrEmpty(Nif));
            Contract.Invariant(IdTipoFuncionario > 0);
            Contract.Invariant(Contactos != null);
        }
    }
}