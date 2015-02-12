using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Domain.Mensagens.Eventos;
using Domain.Relatorios;
using NHibernate;

namespace Domain.Handlers.Eventos {
    public class TrataEventosFuncionarios : ITrataEventosFuncionarios {
        private readonly ISession _session;

        public TrataEventosFuncionarios(ISession session) {
            Contract.Requires(session != null);
            Contract.Ensures(_session != null);
            _session = session;
        }

        public Task Trata(FuncionarioCriado evento) {
            var funcionario = new Funcionario {
                                                  Id = evento.Id,
                                                  Nome = evento.Nome,
                                                  Nif = evento.Nif,
                                                  TipoFuncionario = _session.Load<TipoFuncionario>(evento.IdTipoFuncionario),
                                                  Versao = evento.Versao
                                              };
            _session.Save(funcionario);
            return Task.FromResult(0);
        }

        public Task Trata(DadosGeraisFuncionarioModificados evento) {
            var funcionario = _session.Load<Funcionario>(evento.Id);
            funcionario.Nome = evento.Nome;
            funcionario.Nif = evento.Nif;
            funcionario.TipoFuncionario = _session.Load<TipoFuncionario>(evento.IdTipoFuncionario);
            funcionario.Versao = evento.Versao;
            _session.Save(funcionario);
            return Task.FromResult(0);
        }

        public Task Trata(ContactosFuncionarioModificados evento) {
            var funcionario = _session.Load<Funcionario>(evento.Id);
            funcionario.Contactos = funcionario.Contactos.Where(c => !evento.ContactosRemover.Contains(c)).ToList();
            funcionario.Contactos = funcionario.Contactos.Union(evento.ContactosAdicionar).ToList();
            funcionario.Versao = evento.Versao;
            return Task.FromResult(0);
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_session != null);
        }
    }
}