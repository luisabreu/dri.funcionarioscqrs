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

        private async Task VerificaDadosGerais(string nif, Guid id, int idTipoFuncionario) {
            if (await _verificadorNif.NifDuplicado(nif, id))
            {
                throw new InvalidOperationException(Msg.Nif_duplicado);
            }
            if (!_verificadorTiposFuncionario.TipoFuncionarioValido(idTipoFuncionario))
            {
                throw new InvalidOperationException(Msg.Tipo_funcionario_invalido);
            }
        }

        public async Task Trata(CriaFuncionario comando) {
            await VerificaDadosGerais(comando.Nif, comando.Id, comando.IdTipoFuncionario);

            var funcionario = new Funcionario(comando);
            await _repositorio.Grava(funcionario, ExpectedVersion.NoStream);
        }

        public async Task Trata(ModificaDadosGeraisFuncionario comando) {
            await VerificaDadosGerais(comando.Nif, comando.Id, comando.IdTipoFuncionario);

            var funcionario = await _repositorio.Obtem(comando.Id);
            if (funcionario == null) {
                throw new InvalidOperationException(Msg.Funcionario_inexistente);
            }
            funcionario.ModificaDadosGerais(comando);
            await _repositorio.Grava(funcionario, comando.Versao);
        }

        public async Task Trata(ModificaContactosFuncionario comando) {
            var funcionario = await _repositorio.Obtem(comando.Id);
            if (funcionario == null) {
                throw new InvalidOperationException(Msg.Funcionario_inexistente);
            }
            funcionario.ModificaContactos(comando);
            await _repositorio.Grava(funcionario, comando.Versao);
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