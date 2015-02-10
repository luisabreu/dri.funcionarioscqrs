using System;
using System.Diagnostics.Contracts;

namespace Domain.Mensagens.Eventos {
    public class DadosGeraisFuncionarioModificados : IEvento {
        public readonly int IdTipoFuncionario;
        public readonly string Nif;
        public readonly string Nome;

        public DadosGeraisFuncionarioModificados(Guid id,
            string nif,
            string nome,
            int idTipoFuncionario) {
            Contract.Requires(id != Guid.Empty);
            Contract.Requires(!string.IsNullOrEmpty(nome));
            Contract.Requires(!string.IsNullOrEmpty(nif));
            Contract.Requires(idTipoFuncionario > 0);
            Contract.Ensures(Id != Guid.Empty);
            Contract.Ensures(!string.IsNullOrEmpty(Nome));
            Contract.Ensures(!string.IsNullOrEmpty(Nif));
            Contract.Ensures(IdTipoFuncionario > 0);
            Id = id;
            IdTipoFuncionario = idTipoFuncionario;
            Nif = nif;
            Nome = nome;
        }

        public Guid Id { get; private set; }
        public int Versao { get; set; }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(Id != Guid.Empty);
            Contract.Invariant(!string.IsNullOrEmpty(Nome));
            Contract.Invariant(!string.IsNullOrEmpty(Nif));
            Contract.Invariant(IdTipoFuncionario > 0);
        }
    }
}