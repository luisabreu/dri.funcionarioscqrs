using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using Domain.Repositorios;
using NHibernate;
using NHibernate.Transform;

namespace Domain.Relatorios {
    internal class GestorRelatorios : IGestorRelatorios {
        private static Regex _nifRegex = new Regex(@"^\d{9}$");
        private readonly IRepositorioTiposFuncionario _repositorioTiposFuncionario;
        private readonly ISession _session;

        public GestorRelatorios(IRepositorioTiposFuncionario repositorioTiposFuncionario,
            ISession session) {
            Contract.Requires(repositorioTiposFuncionario != null);
            Contract.Requires(session != null);
            Contract.Ensures(_repositorioTiposFuncionario != null);
            Contract.Ensures(_session != null);
            _repositorioTiposFuncionario = repositorioTiposFuncionario;
            _session = session;
        }

        public IEnumerable<TipoFuncionario> ObtemTodosTiposFuncionarios() {
            return _repositorioTiposFuncionario.ObtemTiposFuncionario()
                .Select(t => new TipoFuncionario {IdTipoFuncionario = t.Id, Descricao = t.Descricao})
                .ToList();
        }

        public IEnumerable<ResumoFuncionario> Pesquisa(string nifOuNome) {
            const string sql =
                "select Id, Nome, Nif, descricao as TipoFuncionario from Funcionarios f inner join TipoFuncinario tf on f.IdTipofuncionario=tf.Id where {0} like '%:str%";
            var items = _session.CreateSQLQuery(string.Format(sql, ENif(nifOuNome) ? " nif " : " nome "))
                .SetString("str", nifOuNome.Replace(' ', '%'))
                .SetResultTransformer(Transformers.AliasToBean<ResumoFuncionario>())
                .List<ResumoFuncionario>();
            return items;
        }

        private bool ENif(string nomeOuNif) {
            return _nifRegex.IsMatch(nomeOuNif);
        }

        public Funcionario Obtem(Guid idFuncionario) {
            return _session.Load<Funcionario>(idFuncionario);
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_repositorioTiposFuncionario != null);
            Contract.Invariant(_session != null);
        }
    }
}