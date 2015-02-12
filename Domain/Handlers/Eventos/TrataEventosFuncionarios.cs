using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Domain.Mensagens.Eventos;
using Domain.Relatorios;
using NHibernate;

namespace Domain.Handlers.Eventos {
    public static class TrataEventosFuncionarios {
      
        public static void Trata(FuncionarioCriado evento, ISession session) {
            Contract.Requires(evento != null);
            Contract.Requires(session != null);
            var funcionario = new Funcionario {
                                                  Id = evento.Id,
                                                  Nome = evento.Nome,
                                                  Nif = evento.Nif,
                                                  TipoFuncionario = session.Load<TipoFuncionario>(evento.IdTipoFuncionario),
                                                  Versao = evento.Versao
                                              };
            session.Save(funcionario);
        }

        public static void Trata(DadosGeraisFuncionarioModificados evento, ISession session) {
            Contract.Requires(evento != null);
            Contract.Requires(session != null);
            var funcionario = session.Load<Funcionario>(evento.Id);
            funcionario.Nome = evento.Nome;
            funcionario.Nif = evento.Nif;
            funcionario.TipoFuncionario = session.Load<TipoFuncionario>(evento.IdTipoFuncionario);
            funcionario.Versao = evento.Versao;
            session.Update(funcionario);
        }

        public static void Trata(ContactosFuncionarioModificados evento, ISession session) {
            Contract.Requires(evento != null);
            Contract.Requires(session != null);
            var funcionario = session.Load<Funcionario>(evento.Id);
            funcionario.Contactos = funcionario.Contactos.Where(c => !evento.ContactosRemover.Contains(c)).ToList();
            funcionario.Contactos = funcionario.Contactos.Union(evento.ContactosAdicionar).ToList();
            funcionario.Versao = evento.Versao;
            session.Update(funcionario);
        }

    }
}