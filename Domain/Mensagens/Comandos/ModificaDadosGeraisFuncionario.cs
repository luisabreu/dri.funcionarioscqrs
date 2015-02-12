using System;
using System.Diagnostics.Contracts;

namespace Domain.Mensagens.Comandos {
    public class ModificaDadosGeraisFuncionario : IComando {
        public readonly int IdTipoFuncionario;
        public readonly string Nif;
        public readonly string Nome;

        public ModificaDadosGeraisFuncionario(Guid id,
            int versao,
            string nome,
            string nif,
            int idTipoFuncionario) {
            Contract.Requires(id != Guid.Empty);
            Contract.Requires(!string.IsNullOrEmpty(nome));
            Contract.Requires(!string.IsNullOrEmpty(nif));
            Contract.Requires(idTipoFuncionario > 0);
            Contract.Ensures(Id != Guid.Empty);
            Contract.Ensures(!string.IsNullOrEmpty(Nome));
            Contract.Ensures(!string.IsNullOrEmpty(Nif));
            Contract.Ensures(IdTipoFuncionario > 0);
            if (!VerificadorNif.NifValido(nif)) {
                throw new ArgumentException(Msg.Nif_invalido);
            }
            IdTipoFuncionario = idTipoFuncionario;
            Nif = nif;
            Nome = nome;
            Id = id;
            Versao = versao;
        }

        public Guid Id { get; private set; }
        public int Versao { get; private set; }

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