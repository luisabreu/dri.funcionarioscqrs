using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Domain.Handlers;
using Domain.Mensagens.Comandos;
using Domain.Relatorios;
using Domain.VO;
using NHibernate;
using site.Models;

namespace site.Controllers {
    public class FuncionariosController : Controller {
        private readonly IGestorRelatorios _gestorRelatorios;
        private readonly ITrataComandosFuncionario _processador;
        private readonly ISession _session;

        public FuncionariosController(ISession session, IGestorRelatorios gestorRelatorios, ITrataComandosFuncionario processador) {
            Contract.Requires(session != null);
            Contract.Requires(gestorRelatorios != null);
            Contract.Requires(processador != null);
            Contract.Ensures(_session != null);
            Contract.Ensures(_gestorRelatorios != null);
            Contract.Ensures(_processador != null);
            _session = session;
            _gestorRelatorios = gestorRelatorios;
            _processador = processador;
        }

        public ActionResult Index() {
            return View(new DadosPesquisa {NifOuNome = "", Funcionarios = Enumerable.Empty<ResumoFuncionario>(), PesquisaEfetuada = false});
        }

        public ActionResult Pesquisa(string nifOuNome) {
            Contract.Requires(!string.IsNullOrEmpty(nifOuNome), Msg.String_vazia);
            using (var tran = _session.BeginTransaction()) {
                var funcionarios = _gestorRelatorios.Pesquisa(nifOuNome);
                return View("Index", new DadosPesquisa {NifOuNome = nifOuNome, Funcionarios = funcionarios, PesquisaEfetuada = true});
            }
        }

        public ActionResult Funcionario(Guid? id) {
            using (var tran = _session.BeginTransaction()) {
                var tipos = _gestorRelatorios.ObtemTodosTiposFuncionarios();
                var func = id.HasValue ?
                    _gestorRelatorios.Obtem(id.Value) :
                    CriaFuncionarioDtoVazio(tipos);


                return View(new DadosFormularioFuncionario {Funcionario = func, TiposFuncionario = tipos, Novo = func == null || Novo(func)});
            }
        }

        private static Funcionario CriaFuncionarioDtoVazio(IEnumerable<TipoFuncionario> tipos) {
            return new Funcionario {
                                       Contactos = new List<Contacto>(),
                                       Nif = "",
                                       Nome = "",
                                       TipoFuncionario = tipos.First()
                                   };
        }

        [HttpPost]
        public async Task<ActionResult> DadosGerais(Guid id, int versao, string nome, string nif, int tipoFuncionario) {
            var criarNovoFuncionario = id == Guid.Empty && versao == 0;
            IEnumerable<TipoFuncionario> tipos = null;
            Funcionario funcionario = null;
            var novo = true;

            using (var tran = _session.BeginTransaction()) {
                try {
                    tipos = _session.QueryOver<TipoFuncionario>().List<TipoFuncionario>();
                    var tipo = tipos.FirstOrDefault(t => t.IdTipoFuncionario == tipoFuncionario);
                    Contract.Assert(tipo != null, Msg.Tipo_funcionario_inexistente);

                    if (!criarNovoFuncionario) {
                        var comando = new ModificaDadosGeraisFuncionario(id, versao, nome, nif, tipo.IdTipoFuncionario);
                        await _processador.Trata(comando);
                    }
                    else {
                        var comando = new CriaFuncionario(Guid.NewGuid(), nome, nif, tipo.IdTipoFuncionario);
                        await _processador.Trata(comando);
                    }

                    tran.Commit();
                }
                catch (Exception ex) {
                    ModelState.AddModelError("total", ex.Message);
                }
            }
            return View("Funcionario", new DadosFormularioFuncionario {
                                                                          Funcionario = !criarNovoFuncionario || !novo ? _session.Load<Funcionario>(id) : CriaFuncionarioDtoVazio(tipos),
                                                                          Novo = criarNovoFuncionario && novo,
                                                                          TiposFuncionario = tipos
                                                                      });
        }

        [HttpPost]
        public ActionResult EliminaContacto(Guid id, int versao, string contacto) {
            IEnumerable<TipoFuncionario> tipos = null;
            MsgGravacao msg = null;
            using (var tran = _session.BeginTransaction()) {
                try {
                    tipos = _session.QueryOver<TipoFuncionario>().List<TipoFuncionario>();
                    var ct = Contacto.Parses(contacto);
                    Contract.Assert(ct != null, Msg.Contacto_invalido);
                    var cmd = new ModificaContactosFuncionario(id, versao, new[] {ct}, null);

                    msg = _processador.Trata(cmd);
                    tran.Commit();
                }
                catch (Exception ex) {
                    ModelState.AddModelError("total", ex.Message);
                }
            }
            return View("Funcionario", new DadosFormularioFuncionario {
                                                                          Funcionario = _session.Load<Funcionario>(id),
                                                                          Novo = false,
                                                                          TiposFuncionario = tipos
                                                                      });
        }

        [HttpPost]
        public ActionResult AdicionaContacto(Guid id, int versao, string contacto) {
            IEnumerable<TipoFuncionario> tipos = null;
            MsgGravacao msg = null;
            using (var tran = _session.BeginTransaction()) {
                try {
                    tipos = _session.QueryOver<TipoFuncionario>().List<TipoFuncionario>();
                    var ct = Contacto.Parses(contacto);
                    Contract.Assert(ct != null, Msg.Contacto_invalido);
                    var cmd = new ModificaContactosFuncionario(id, versao, null, new[] {ct});

                    msg = _processador.Trata(cmd);
                    tran.Commit();
                }
                catch (Exception ex) {
                    ModelState.AddModelError("total", ex.Message);
                }
            }
            return View("Funcionario", new DadosFormularioFuncionario {
                                                                          Funcionario = _session.Load<Funcionario>(id),
                                                                          Novo = false,
                                                                          TiposFuncionario = tipos
                                                                      });
        }

        private static bool Novo(Funcionario funcionario) {
            return funcionario != null && funcionario.Id == Guid.Empty && funcionario.Versao == 0;
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_session != null);
            Contract.Invariant(_gestorRelatorios != null);
            Contract.Invariant(_processador != null);
        }
    }
}