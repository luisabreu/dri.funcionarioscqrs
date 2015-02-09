using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Domain.Mensagens;

namespace Domain.Agregados {
    public abstract class Agregado {
        private readonly IList<IEvento> _eventos = new List<IEvento>();

        public abstract Guid Id { get; }

        public IEnumerable<IEvento> ObtemEventosNaoPersistidos() {
            return _eventos;
        }

        public void MarcaEventosComoGravados() {
            _eventos.Clear();
        }

        public void CarregaDeHistorico(IEnumerable<IEvento> eventos) {
            Contract.Requires(eventos != null);
            foreach (var evento in eventos) {
                AplicaEvento(evento, false);
            }
        }

        protected void AplicaEvento(IEvento evento) {
            AplicaEvento(evento, true);
        }

        private void AplicaEvento(IEvento evento, bool eventoNovo) {
            Contract.Requires(evento != null);
            dynamic @this = new AuxHelperDynamic(this);
            @this.Aplica(evento);
            if (eventoNovo) {
                _eventos.Add(evento);
            }
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(_eventos != null);
        }

        public class AuxHelperDynamic : DynamicObject {
            private object _wrapped;

            public AuxHelperDynamic(Object wrapped) {
                _wrapped = wrapped;
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
                var methodInfo = _wrapped.GetType()
                    .GetMethod(binder.Name,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, args.Select(a => a.GetType()).ToArray(), null);
                result = methodInfo.Invoke(_wrapped, args);
                return true;
            }
        }
    }
}