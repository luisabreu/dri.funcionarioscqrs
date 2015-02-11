using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Domain.Mensagens.Comandos;

namespace Domain.Handlers {
    [ContractClass(typeof (ContratoTrataComandosFuncionario))]
    public interface ITrataComandosFuncionario {
        Task Trata(CriaFuncionario comando);
    }

    [ContractClassFor(typeof (ITrataComandosFuncionario))]
    internal abstract class ContratoTrataComandosFuncionario:ITrataComandosFuncionario {
        public Task Trata(CriaFuncionario mensagem) {
            Contract.Requires(mensagem != null);
            return default(Task);
        }
    }
}