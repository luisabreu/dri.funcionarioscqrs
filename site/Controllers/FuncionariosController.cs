using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Domain.Handlers;
using Domain.Handlers.Comandos;
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
                    CriaFuncionarioDtoVazio(tipos.First());


                return View(new DadosFormularioFuncionario {Funcionario = func, TiposFuncionario = tipos, Novo = func == null || Novo(func)});
            }
        }

        private static Funcionario CriaFuncionarioDtoVazio(TipoFuncionario tipoFuncionario) {
            return new Funcionario {
                                       Contactos = new List<Contacto>(),
                                       Nif = "",
                                       Nome = "",
                                       TipoFuncionario = tipoFuncionario
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
                }
                catch (Exception ex) {
                    novo = false;
                    ModelState.AddModelError("total", ex.Message);
                }
            }

            return View("Funcionario", new DadosFormularioFuncionario {
                Funcionario = !criarNovoFuncionario || !novo ? ObtemFuncionarioComDadosGeraisAtualizados(id, versao + 1, nome, nif, tipos.First(i => i.IdTipoFuncionario == tipoFuncionario)) : CriaFuncionarioDtoVazio(tipos.First()),
                                                                          Novo = criarNovoFuncionario && novo,
                                                                          TiposFuncionario = tipos
                                                                      });
        }

        private Funcionario ObtemFuncionarioComDadosGeraisAtualizados(Guid id, int versaoEsperada, string nome, string nif, TipoFuncionario tipoFuncionario) {
            var funcionario = _gestorRelatorios.Obtem(id);
            Contract.Assert(funcionario != null);
            funcionario.Nome = nome;
            funcionario.Nif = nif;
            funcionario.Versao = versaoEsperada;
            funcionario.TipoFuncionario = tipoFuncionario;
            funcionario.Contactos = funcionario.Contactos ?? new List<Contacto>();
            
            return funcionario;
        }

        private Funcionario ObtemFuncionarioComContactosAtualizados(Guid id, int versaoEsperada, Contacto contactoEliminado = null, Contacto contactoAdicionado = null) {
            var funcionario = _gestorRelatorios.Obtem(id);
            Contract.Assert(funcionario != null);
            funcionario.Versao = versaoEsperada;
            if (contactoEliminado != null) {
                if (funcionario.Contactos.Contains(contactoEliminado)) {
                    funcionario.Contactos = funcionario.Contactos.Where(c => c != contactoEliminado).ToList();
                }
            }
            if (contactoAdicionado != null) {
                if (!funcionario.Contactos.Contains(contactoAdicionado)) {
                    funcionario.Contactos = funcionario.Contactos.Union(new[] {contactoAdicionado});
                }
            }
            return funcionario;
        }

        [HttpPost]
        public async Task<ActionResult> EliminaContacto(Guid id, int versao, string contacto) {
            IEnumerable<TipoFuncionario> tipos = null;
            Contacto contactoEliminado = null;
            var ocorreuErro = false;
            using (var tran = _session.BeginTransaction()) {
                try {
                    tipos = _session.QueryOver<TipoFuncionario>().List<TipoFuncionario>();
                    contactoEliminado = Contacto.Parses(contacto);
                    Contract.Assert(contactoEliminado != null, Msg.Contacto_invalido);
                    var cmd = new ModificaContactosFuncionario(id, versao, null, new[] {contactoEliminado});

                    await _processador.Trata(cmd);
                }
                catch (Exception ex) {
                    ocorreuErro = true;
                    ModelState.AddModelError("total", ex.Message);
                }
            }
            return View("Funcionario", new DadosFormularioFuncionario {
                                                                          Funcionario = ObtemFuncionarioComContactosAtualizados(id, ocorreuErro ? versao :  versao + 1, ocorreuErro ? null : contactoEliminado),// _session.Load<Funcionario>(id),
                                                                          Novo = false,
                                                                          TiposFuncionario = tipos
                                                                      });
        }

        [HttpPost]
        public async Task<ActionResult> AdicionaContacto(Guid id, int versao, string contacto) {
            IEnumerable<TipoFuncionario> tipos = null;
            Contacto contactoAdicionar = null;
            var ocorreuErro = false;
            using (var tran = _session.BeginTransaction()) {
                try {
                    tipos = _session.QueryOver<TipoFuncionario>().List<TipoFuncionario>();
                    contactoAdicionar = Contacto.Parses(contacto);
                    Contract.Assert(contactoAdicionar != null, Msg.Contacto_invalido);
                    var cmd = new ModificaContactosFuncionario(id, versao, new[] {contactoAdicionar});

                    await _processador.Trata(cmd);
                    tran.Commit();
                }
                catch (Exception ex) {
                    ocorreuErro = true;
                    ModelState.AddModelError("total", ex.Message);
                }
            }
            return View("Funcionario", new DadosFormularioFuncionario {
                                                                         Funcionario = ObtemFuncionarioComContactosAtualizados(id, ocorreuErro ? versao : versao + 1, null, ocorreuErro ? null : contactoAdicionar),// _session.Load<Funcionario>(id),
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