using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Domain.Mensagens;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;

namespace Domain.Repositorios {
    public class DepositoEventos : IDepositoEventos {
        private const int _numeroEventosPorPagina = 200;
        private readonly IEventStoreConnection _ligacaoDeposito;
        private readonly ISeriadorEventos _seriadorEventos;

        public DepositoEventos(IEventStoreConnection ligacaoDeposito, ISeriadorEventos seriadorEventos) {
            Contract.Requires(ligacaoDeposito != null);
            Contract.Requires(seriadorEventos != null);
            Contract.Ensures(_ligacaoDeposito != null);
            _ligacaoDeposito = ligacaoDeposito;
            _seriadorEventos = seriadorEventos;
        }

        public async Task GravaEventos(string idStream, string tipo, IEnumerable<IEvento> eventosGravar, int versaoEsperada) {
            var eventosPreviamenteGravadosEmConflitoEventual = await ObtemEventosParaAgregado(idStream, versaoEsperada + 1);

            if (eventosGravar.Any(eventoGravar => eventosPreviamenteGravadosEmConflitoEventual.Any(eventoGravado => ExisteConflitoEventos(eventoGravar, eventoGravado)))) {
                throw new ConcurrencyException();
            }
            //update event number
            var evtVersionNumber = eventosPreviamenteGravadosEmConflitoEventual.Any() ? eventosPreviamenteGravadosEmConflitoEventual.Max(evt => evt.Versao) : versaoEsperada;
            /*foreach (var evento in eventosGravar) {
                evento.Versao = ++evtVersionNumber;
            }*/
            try {
                using (var tran = await _ligacaoDeposito.StartTransactionAsync(idStream, evtVersionNumber)) {
                    await tran.WriteAsync(
                        eventosGravar.Select(eventoGravar => new EventData(Guid.NewGuid(),
                            eventoGravar.GetType().Name.ToLower(),
                            true,
                            _seriadorEventos.SeriaEvento(eventoGravar),
                            null)));
                    await tran.CommitAsync();
                }
            }
            catch (WrongExpectedVersionException ex) {
                throw new ConcurrencyException("", ex);
            }
        }

        public async Task<IEnumerable<IEvento>> ObtemEventosParaAgregado(string idStream, int versaoEsperada = StreamPosition.Start) {
            var eventos = new List<IEvento>();
            if (versaoEsperada < 0) {
                return eventos;
            }
            try {
                var indiceProxPaginaElementos = versaoEsperada;
                StreamEventsSlice paginaElementos = null;
                do {
                    paginaElementos = await _ligacaoDeposito.ReadStreamEventsForwardAsync(idStream, indiceProxPaginaElementos, _numeroEventosPorPagina, false);
                    eventos.AddRange(paginaElementos.Events.Select(_seriadorEventos.DeseriaEvento));
                    indiceProxPaginaElementos = paginaElementos.NextEventNumber;
                } while (!paginaElementos.IsEndOfStream);
            }
            catch (Exception ex) {
                var t = "";
            }
            return eventos;
        }

        protected virtual bool ExisteConflitoEventos(IEvento eventoGravar, IEvento eventoGravado) {
            return eventoGravado.GetType() == eventoGravar.GetType();
        }
    }
}