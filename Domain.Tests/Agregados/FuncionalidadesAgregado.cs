using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Agregados;
using Domain.Mensagens;
using FluentAssertions;
using Xbehave;

namespace Domain.Tests.Agregados {
    public class FuncionalidadesAgregado {

        [Scenario]
        public void Criacao(AgregadoDemo agregado, IEnumerable<IEvento> eventos) {
            "Dado um novo agregado"
                .Given(() => agregado = new AgregadoDemo());

            "Quando acedemos aos eventos a gravar"
                .When(() => eventos = agregado.ObtemEventosNaoPersistidos());

            "Devemos obter uma coleção vazia"
                .Then(() => eventos.Should().BeEmpty());
        }

        [Scenario]
        public void Depois_criar_um_agregado(AgregadoDemo agregado) {
            "Dado um agregado inicializado"
                .Given(() => agregado = new AgregadoDemo());

            "Quando executamos um comando"
                .When(() => agregado.ExecutaUmComando());

            "Então devemos obter um evento nos eventos a gravar"
                .Then(() => agregado.ObtemEventosNaoPersistidos().Any(e => e.GetType() == typeof (EventoDemo)));
        }



        public class AgregadoDemo : Agregado {
            private readonly Guid _guid = Guid.NewGuid();
            private EventoDemo _evento;

            public override Guid Id {
                get { return _guid; }
            }

            public void ExecutaUmComando() {
                AplicaEvento(new EventoDemo());
            }

            private void AplicaEvento(EventoDemo evento) {
                _evento = evento;
            }
        }

        public class EventoDemo:IEvento{
            public Guid IdAgregado { get; private set; }

            public int Versao { get; set; }
        }
    }
}