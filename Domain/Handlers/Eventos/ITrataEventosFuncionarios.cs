using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Domain.Mensagens.Eventos;

namespace Domain.Handlers.Eventos {
    [ContractClass(typeof(ContratoTrataEventosFuncionarios))]
    public interface ITrataEventosFuncionarios {
        Task Trata(FuncionarioCriado evento);
        Task Trata(DadosGeraisFuncionarioModificados evento);
        Task Trata(ContactosFuncionarioModificados evento);
    }

    [ContractClassFor(typeof(ITrataEventosFuncionarios))]
    abstract class ContratoTrataEventosFuncionarios:ITrataEventosFuncionarios {
        public Task Trata(FuncionarioCriado evento) {
            Contract.Requires(evento != null);
            return default (Task);
        }

        public Task Trata(DadosGeraisFuncionarioModificados evento) {
            Contract.Requires(evento != null);
            return default(Task);
        }

        public Task Trata(ContactosFuncionarioModificados evento) {
            Contract.Requires(evento != null);
            return default(Task);
        }
    }
}