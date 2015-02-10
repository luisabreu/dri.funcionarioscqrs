using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Domain.Mensagens.Comandos;
using Domain.Mensagens.Eventos;
using Domain.VO;

namespace Domain.Agregados {
    public class Funcionario : Agregado {
        private List<Contacto> _contactos;
        private Guid _id;

        public Funcionario(CriaFuncionario comando) {
            Contract.Requires(comando != null);
            var evento = new FuncionarioCriado(comando.Id, comando.Nome, comando.Nif, comando.IdTipoFuncionario);
            AplicaEvento(evento);
        }

        internal Funcionario() {
        }

        public override Guid Id {
            get { return _id; }
        }

        public void Executa(ModificaDadosGeraisFuncionario comando) {
            Contract.Requires(comando != null);
            var evento = new DadosGeraisFuncionarioModificados(comando.Id, 
                comando.Nif, 
                comando.Nome,
                comando.IdTipoFuncionario);
            AplicaEvento(evento);
        }

        private void Aplica(FuncionarioCriado evento) {
            Contract.Requires(evento != null);
            _id = evento.Id;
            _contactos = evento.Contactos.ToList();
        }

        private void Aplica(DadosGeraisFuncionarioModificados evento) {
            Contract.Requires(evento != null);
            //do nothing
        }
    }
}