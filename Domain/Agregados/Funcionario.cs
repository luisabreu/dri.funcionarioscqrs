using System;
using System.Diagnostics.Contracts;
using Domain.Mensagens;
using Domain.Mensagens.Comandos;

namespace Domain.Agregados {
    public class Funcionario : Agregado {
        private Guid _id;

        public override Guid Id {
            get { return _id; }
        }

        public Funcionario(CriaFuncionario comando) {
            Contract.Requires(comando != null);
            _id = comando.Id;
        }

        private void Aplica(FuncionarioCriado cmd) {
            Contract.Requires(cmd != null);
        }
        internal Funcionario(){}
    }
}