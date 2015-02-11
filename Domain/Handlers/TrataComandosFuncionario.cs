using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Domain.Agregados;
using Domain.Mensagens.Comandos;
using Domain.Repositorios;
using Domain.Servicos;
using EventStore.ClientAPI;

namespace Domain.Handlers {
    public class TrataComandosFuncionario : ITrataComandosFuncionario {
        private readonly IRepositorioFuncionarios _repositorio;
        private readonly IServicoDuplicacaoNif _verificadorNif;
        private readonly IServicoVerificacaoTiposFuncionario _verificadorTiposFuncionario;

        public TrataComandosFuncionario(IRepositorioFuncionarios repositorio, IServicoDuplicacaoNif verificadorNif, IServicoVerificacaoTiposFuncionario verificadorTiposFuncionario) {
            Contract.Requires(repositorio != null);
            Contract.Requires(verificadorNif != null);
            Contract.Requires(verificadorTiposFuncionario != null);
            Contract.Ensures(_repositorio != null);
            Contract.Ensures(_verificadorNif != null);
            Contract.Ensures(_verificadorTiposFuncionario != null);
            _repositorio = repositorio;
            _verificadorNif = verificadorNif;
            _verificadorTiposFuncionario = verificadorTiposFuncionario;
        }

        public Task Trata(CriaFuncionario comando) {
            if (_verificadorNif.NifDuplicado(comando.Nif, comando.Id)) {
                throw new InvalidOperationException(Msg.Nif_duplicado);
            }
            if (!_verificadorTiposFuncionario.TipoFuncionarioValido(comando.IdTipoFuncionario)) {
                throw new InvalidOperationException(Msg.Tipo_funcionario_invalido);
            }

            var funcionario = new Funcionario(comando);
            return _repositorio.Grava(funcionario, ExpectedVersion.NoStream);
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_repositorio != null);
            Contract.Invariant(_verificadorNif != null);
            Contract.Invariant(_verificadorTiposFuncionario != null);
        }
    }
}