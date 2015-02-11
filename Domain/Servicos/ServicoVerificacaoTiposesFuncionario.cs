using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using Domain.Repositorios;

namespace Domain.Servicos {
    public class ServicoVerificacaoTiposesFuncionario : IServicoVerificacaoTiposFuncionario {
        private readonly RepositorioTiposFuncionario _repositorio;

        public ServicoVerificacaoTiposesFuncionario(RepositorioTiposFuncionario repositorio) {
            Contract.Requires(repositorio != null);
            Contract.Ensures(_repositorio != null);
            _repositorio = repositorio;
        }

        public bool TipoFuncionarioValido(int idTipoFuncionario) {
            return _repositorio.ObtemTiposFuncionario().Any(tf => tf.Id == idTipoFuncionario);
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_repositorio != null);
        }
    }
}