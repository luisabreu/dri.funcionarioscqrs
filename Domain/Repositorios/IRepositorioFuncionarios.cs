using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Domain.Agregados;

namespace Domain.Repositorios {
    [ContractClass(typeof (ContratoRepositorioFuncionarios))]
    public interface IRepositorioFuncionarios {
        Task Grava(Funcionario funcionario, int versaoEsperada);
        Task<Funcionario> Obtem(Guid id);
    }

    [ContractClassFor(typeof (IRepositorioFuncionarios))]
    internal abstract class ContratoRepositorioFuncionarios : IRepositorioFuncionarios {
        public Task Grava(Funcionario funcionario, int versaoEsperada) {
            Contract.Requires(funcionario != null);
            return default(Task);
        }

        public Task<Funcionario> Obtem(Guid id) {
            Contract.Requires(id != Guid.Empty);
            return default(Task<Funcionario>);
        }
    }
}