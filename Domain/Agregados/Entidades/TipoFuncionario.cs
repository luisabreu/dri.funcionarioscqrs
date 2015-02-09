using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Domain.Agregados.Entidades {
    public class TipoFuncionario {
        public TipoFuncionario(int id, string descricao) {
            Contract.Requires(!string.IsNullOrEmpty(descricao));
            Contract.Ensures(!string.IsNullOrEmpty(Descricao));
            Id = id;
            Descricao = descricao;
        }

        public string Descricao { get; private set; }
        public int Id { get; private set; }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(!string.IsNullOrEmpty(Descricao));
        }


    }
}