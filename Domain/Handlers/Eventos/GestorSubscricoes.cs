using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using Domain.Mensagens;
using Domain.Mensagens.Eventos;
using Domain.Repositorios;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using NHibernate;

namespace Domain.Handlers.Eventos {
    public class GestorSubscricoes {
        private const int _id = 1;
        private static object _locker = new object();
        private readonly IEventStoreConnection _cnn;
        private readonly UserCredentials _credenciais;
        private readonly ISeriadorEventos _seriadorEventos;
        private readonly ISessionFactory _sessionFactory;
        private PosicaoStream _posicaoStreamBd;
        private EventStoreAllCatchUpSubscription _subscricao;

        private IDictionary<Type, Action<IEvento, ISession>> _subscritores =
            new Dictionary<Type, Action<IEvento, ISession>> {
                                                                {
                                                                    typeof (FuncionarioCriado),
                                                                    (evt, sess) => TrataEventosFuncionarios.Trata((FuncionarioCriado) evt, sess)
                                                                }, {
                                                                       typeof (DadosGeraisFuncionarioModificados),
                                                                       (evt, sess) => TrataEventosFuncionarios.Trata((DadosGeraisFuncionarioModificados) evt, sess)
                                                                   }, {
                                                                          typeof (ContactosFuncionarioModificados),
                                                                          (evt, sess) => TrataEventosFuncionarios.Trata((ContactosFuncionarioModificados) evt, sess)
                                                                      }
                                                            };

        public GestorSubscricoes(IEventStoreConnection cnn,
            ISessionFactory sessionFactory,
            UserCredentials credenciais,
            ISeriadorEventos seriadorEventos) {
            Contract.Requires(cnn != null);
            Contract.Requires(credenciais != null);
            Contract.Requires(sessionFactory != null);
            Contract.Requires(seriadorEventos != null);
            Contract.Ensures(_cnn != null);
            Contract.Ensures(_credenciais != null);
            Contract.Ensures(_sessionFactory != null);
            Contract.Ensures(_seriadorEventos != null);

            _cnn = cnn;
            _sessionFactory = sessionFactory;
            _credenciais = credenciais;
            _seriadorEventos = seriadorEventos;
        }

        private void InicializaPosicaoAtual() {
            lock (_locker) {
                using (var session = _sessionFactory.OpenStatelessSession()) {
                    using (var tran = session.BeginTransaction()) {
                        _posicaoStreamBd = session.Get<PosicaoStream>(_id) ?? new PosicaoStream{IdPosicaoStream = _id};
                    }
                }
            }
        }

        private void AtualizaPosicaoInterna(Position? posicao, ISession session) {
            lock (_locker) {
                _posicaoStreamBd.PosicaoCommit = posicao.Value.CommitPosition;
                _posicaoStreamBd.PosicaoPreparacao = posicao.Value.PreparePosition;
                session.SaveOrUpdate(_posicaoStreamBd);
            }
        }

        public void Subscreve() {
            InicializaPosicaoAtual();
            Position posicalInicial;
            lock (_locker) {
                posicalInicial = new Position(_posicaoStreamBd.PosicaoCommit, _posicaoStreamBd.PosicaoPreparacao);
                _subscricao = _cnn.SubscribeToAllFrom(posicalInicial,
                    true,
                    TrataEvento,
                    null,
                    TrataQuedaLigacao,
                    _credenciais);
            }
        }

        public void CancelaSubscricao() {
            lock (_locker) {
                _subscricao.Stop(TimeSpan.FromMilliseconds(5000));
            }
        }

        private void TrataEvento(EventStoreCatchUpSubscription subscricao, ResolvedEvent evento) {
            Guid aux;
            if (!Guid.TryParse(evento.OriginalStreamId, out aux)) {
                return;
            }

            using (var session = _sessionFactory.OpenSession()) {
                using (var tran = session.BeginTransaction()) {
                    try {
                        if (!evento.OriginalEvent.EventType.StartsWith("$") &&
                            !evento.OriginalStreamId.StartsWith("$")) {
                            var eventoDeseriado = _seriadorEventos.DeseriaEvento(evento);
                            _subscritores[eventoDeseriado.GetType()](eventoDeseriado, session);
                        }
                    }
                    catch (Exception ex) {
                        //swallow
                    }
                    AtualizaPosicaoInterna(evento.OriginalPosition, session);
                    tran.Commit();
                }
            }
        }

        private void TrataQuedaLigacao(EventStoreCatchUpSubscription subscricao, SubscriptionDropReason razao,
            Exception ex) {
            if (razao == SubscriptionDropReason.UserInitiated) {
                return;
            }
            Subscreve();
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_cnn != null);
            Contract.Invariant(_credenciais != null);
            Contract.Invariant(_sessionFactory != null);
            Contract.Invariant(_seriadorEventos != null);
        }
    }
}