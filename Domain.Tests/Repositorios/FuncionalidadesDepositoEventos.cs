﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Mensagens;
using Domain.Repositorios;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Xunit;

namespace Domain.Tests.Repositorios {
    public class FuncionalidadesDepositoEventos {
        private const string _eventStorePath = @"E:\tools\EventStore\eventstore.clusternode.exe";
        private const string _args = @"--mem-db=true --ext-tcp-port={0} --ext-http-port={1} --run-projections=all";
        private const string _tipoAgregado = "testes";
        private static JsonSerializerSettings _definicoesJson = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects};

        private Process IniciaEventStore(string tcpPort, string httpPort) {
            return Process.Start(_eventStorePath, string.Format(_args, tcpPort, httpPort));
        }

        private void FechaEventStore(Process process) {
            process.Kill();
        }

        [Fact]
        public async Task Guarda_eventos_novo_agregado() {
            IEventStoreConnection ligacao = null;
            var stream = Guid.NewGuid();
            Process listener = null;
            try {
                listener = IniciaEventStore("1120", "2120");
                ligacao = EventStoreConnection.Create( new IPEndPoint(IPAddress.Loopback, 1120));
                var deposito = new DepositoEventos(ligacao, new SeriadorEventos());

                var eventos = new List<IEvento> {
                                                    new EventoTeste(stream, 0),
                                                    new EventoTeste(stream, 0),
                                                    new EventoTeste(stream, 0)
                                                };
                await ligacao.ConnectAsync();
                await deposito.GravaEventos(stream.ToString(), _tipoAgregado, eventos, ExpectedVersion.NoStream);
                var slice = await ligacao.ReadStreamEventsForwardAsync(stream.ToString(), StreamPosition.Start, eventos.Count(), false);
                var lidos = slice.Events
                    .Select(DeseriaEvento)
                    .ToList();

                Assert.Equal(3, lidos.Count());

                var idsEventos = eventos.Select(e => e.Id).ToList();
                Assert.True(idsEventos.All(id => lidos.Exists(l => l.Id == id)));
                Assert.True(new[] {0, 1, 2}.All(versao => lidos.Exists(l => l.Versao == versao)));
                await ligacao.DeleteStreamAsync(stream.ToString(), ExpectedVersion.Any);
            }
            finally {
                ligacao.Close();
                if (listener != null) {
                    FechaEventStore(listener);
                }
            }
        }

        [Fact]
        public async Task Carrega_eventos_agregado() {
            IEventStoreConnection ligacao = null;
            var stream = Guid.NewGuid();
            Process listener = null;
            try {
                listener = IniciaEventStore("1121", "2121");
                ligacao = EventStoreConnection.Create( new IPEndPoint(IPAddress.Loopback, 1121));
                var deposito = new DepositoEventos(ligacao, new SeriadorEventos());

                var eventos = new List<IEvento> {
                                                    new EventoTeste(stream, 0),
                                                    new EventoTeste(stream, 0),
                                                    new EventoTeste(stream, 0)
                                                };
                await ligacao.ConnectAsync();
                await ligacao.AppendToStreamAsync(stream.ToString(),
                    ExpectedVersion.NoStream,
                    eventos.Select(eventoGravar => new EventData(Guid.NewGuid(),
                        eventoGravar.GetType().Name.ToLower(),
                        true,
                        SeriaEvento(eventoGravar),
                        null)));

                var eventosObtidos = (await deposito.ObtemEventosParaAgregado(stream.ToString())).ToList();

                Assert.Equal(3, eventosObtidos.Count());

                var idsEventos = eventos.Select(e => e.Id).ToList();
                Assert.True(idsEventos.All(id => eventosObtidos.Exists(l => l.Id == id)));
                Assert.True(new[] {0, 1, 2}.All(versao => eventosObtidos.Exists(l => l.Versao == versao)));

            }
            finally {
                
                ligacao.Close();
                if (listener != null) {
                    FechaEventStore(listener);
                }
            }
        }

        [Fact]
        public async Task Adiciona_eventos_agregado() {
            IEventStoreConnection ligacao = null;
            var stream = Guid.NewGuid();
            Process listener = null;
            try {
                listener = IniciaEventStore("1122", "2122");
                ligacao = EventStoreConnection.Create( new IPEndPoint(IPAddress.Loopback, 1122));
                var deposito = new DepositoEventos(ligacao, new SeriadorEventos());

                var eventos = new List<IEvento> {
                                                    new EventoTeste(stream, 0),
                                                    new EventoTeste(stream, 0),
                                                    new EventoTeste(stream, 0)
                                                };

                var novosEventos = new List<IEvento> {
                                                         new EventoTeste(stream, 0),
                                                         new EventoTeste(stream, 0)
                                                     };
                await ligacao.ConnectAsync();
                await ligacao.AppendToStreamAsync(stream.ToString(),
                    ExpectedVersion.NoStream,
                    eventos.Select(eventoGravar => new EventData(Guid.NewGuid(),
                        eventoGravar.GetType().Name.ToLower(),
                        true,
                        SeriaEvento(eventoGravar),
                        null)));

                await deposito.GravaEventos(stream.ToString(), _tipoAgregado, novosEventos, 2);
                var slice = await ligacao.ReadStreamEventsForwardAsync(stream.ToString(), StreamPosition.Start, int.MaxValue, false);
                var eventosObtidos = slice.Events.Select(DeseriaEvento).ToList();

                Assert.Equal(5, eventosObtidos.Count());

                var idsEventos = eventos.Select(e => e.Id).ToList();
                Assert.True(idsEventos.All(id => eventosObtidos.Exists(l => l.Id == id)));
                Assert.True(new[] {0, 1, 2, 3, 4}.All(versao => eventosObtidos.Exists(l => l.Versao == versao)));
                await ligacao.DeleteStreamAsync(stream.ToString(), ExpectedVersion.Any);
            }
            finally {
                ligacao.Close();
                if (listener != null) {
                    FechaEventStore(listener);
                }
            }
        }

        private byte[] SeriaEvento(IEvento eventoGravar) {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventoGravar, Formatting.Indented, _definicoesJson));
        }

        private IEvento DeseriaEvento(ResolvedEvent resolvedEvent) {
            var evento = (IEvento) JsonConvert.DeserializeObject(Encoding.UTF8.GetString((resolvedEvent.Event.Data)), _definicoesJson);
            evento.Versao = resolvedEvent.OriginalEventNumber;
            return evento;
        }

        private class EventoTeste : IEvento {
            public EventoTeste(Guid id, int versao) {
                Id = id;
                Versao = versao;
            }

            public Guid Id { get; private set; }
            public int Versao { get; set; }
        }
    }
}