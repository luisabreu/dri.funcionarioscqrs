using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Domain.Comandos;

namespace Domain.Handlers {
    [ContractClass(typeof (ContratoTrataMensagensFuncionario))]
    public interface ITrataMensagensFuncionario {
        Task Trata(CriaFuncionario comando);
    }

    [ContractClassFor(typeof (ITrataMensagensFuncionario))]
    internal abstract class ContratoTrataMensagensFuncionario:ITrataMensagensFuncionario {
        public Task Trata(CriaFuncionario mensagem) {
            Contract.Requires(mensagem != null);
            return default(Task);
        }
    }
}