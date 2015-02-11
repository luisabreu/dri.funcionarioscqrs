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

        public void ModificaDadosGerais(ModificaDadosGeraisFuncionario comando) {
            Contract.Requires(comando != null);
            if (comando.Id != _id) {
                throw new InvalidOperationException(Msg.Comando_incorreto_para_agregadp);
            }
            var evento = new DadosGeraisFuncionarioModificados(comando.Id,
                comando.Nif,
                comando.Nome,
                comando.IdTipoFuncionario);
            AplicaEvento(evento);
        }

        public void ModificaContactos(ModificaContactosFuncionario comando) {
            Contract.Requires(comando != null);
            if (comando.Id != _id) {
                throw new InvalidOperationException(Msg.Comando_incorreto_para_agregadp);
            }
            if (comando.ContactosRemover.Any(c => !_contactos.Contains(c))) {
                throw new InvalidOperationException(Msg.Contactos_eliminar_nao_existem);
            }
            if (comando.ContactosAdicionar.Any(c => _contactos.Contains(c))) {
                throw new InvalidOperationException(Msg.Contactos_adicionar_existem);
            }
            var evento = new ContactosFuncionarioModificados(comando.Id, comando.ContactosAdicionar, comando.ContactosRemover);
            AplicaEvento(evento);
        }

        private void Aplica(FuncionarioCriado evento) {
            Contract.Requires(evento != null);
            _id = evento.Id;
            _contactos = evento.Contactos.ToList();
        }

        private void Aplica(ContactosFuncionarioModificados evento) {
            Contract.Requires(evento != null);
            foreach (var contacto in evento.ContactosRemover) {
                _contactos.Remove(contacto);
            }
            foreach (var contacto in evento.ContactosAdicionar) {
                _contactos.Add(contacto);
            }
        }

        private void Aplica(DadosGeraisFuncionarioModificados evento) {
            Contract.Requires(evento != null);
            //do nothing
        }
    }
}