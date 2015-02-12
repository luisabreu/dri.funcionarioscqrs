using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Domain.Mensagens.Comandos;

namespace Domain.Handlers.Comandos {
    [ContractClass(typeof (ContratoTrataComandosFuncionario))]
    public interface ITrataComandosFuncionario {
        Task Trata(CriaFuncionario comando);

        Task Trata(ModificaDadosGeraisFuncionario comando);

        Task Trata(ModificaContactosFuncionario comando);
    }

    [ContractClassFor(typeof (ITrataComandosFuncionario))]
    internal abstract class ContratoTrataComandosFuncionario:ITrataComandosFuncionario {
        public Task Trata(CriaFuncionario mensagem) {
            Contract.Requires(mensagem != null);
            return default(Task);
        }

        public Task Trata(ModificaDadosGeraisFuncionario comando) {
            Contract.Requires(comando != null);
            return default(Task);
        }

        public Task Trata(ModificaContactosFuncionario comando) {
            Contract.Requires(comando != null);
            return default (Task);
        }
    }
}