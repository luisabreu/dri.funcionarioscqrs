using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Domain.Agregados;

namespace Domain.Repositorios {
    public class RepositorioFuncionarios : IRepositorioFuncionarios {
        private readonly IDepositoEventos _depositoEventos;

        public RepositorioFuncionarios(IDepositoEventos depositoEventos) {
            Contract.Requires(depositoEventos != null);
            Contract.Ensures(_depositoEventos != null);
            _depositoEventos = depositoEventos;
        }

        public async Task Grava(Funcionario funcionario, int versaoEsperada) {
            await _depositoEventos.GravaEventos(funcionario.Id.ToString(), funcionario.GetType().Name.ToLower(), funcionario.ObtemEventosNaoPersistidos(), versaoEsperada);
        }

        public async Task<Funcionario> Obtem(Guid id) {
            var eventos = await _depositoEventos.ObtemEventosParaAgregado(id.ToString());
            var funcionario = new Funcionario();
            funcionario.CarregaDeHistorico(eventos);
            return funcionario;
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_depositoEventos != null);
        }
    }
}