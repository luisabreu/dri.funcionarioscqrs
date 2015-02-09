using System;
using System.Diagnostics.Contracts;
using Domain.Comandos;

namespace Domain.Agregados {
    public class Funcionario : Agregado {
        private Guid _id;

        public override Guid Id {
            get { return _id; }
        }

        public Funcionario(CriaFuncionario comando) {
            Contract.Requires(comando != null);
        }
    }
}