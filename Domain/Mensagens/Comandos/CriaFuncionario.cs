﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using Domain.VO;

namespace Domain.Mensagens.Comandos {
    public class CriaFuncionario : IComando {
        public readonly IEnumerable<Contacto> Contactos;
        public readonly int IdTipoFuncionario;
        public readonly string Nif;
        public readonly string Nome;

        public CriaFuncionario(Guid id,
            string nome,
            string nif,
            int idTipoFuncionario,
            IEnumerable<Contacto> contactos = null) {
            Contract.Requires(!string.IsNullOrEmpty(nome));
            Contract.Requires(!string.IsNullOrEmpty(nif));
            Contract.Ensures(!string.IsNullOrEmpty(Nome));
            Contract.Ensures(Contactos != null);
            Contract.Ensures(!string.IsNullOrEmpty(Nif));
            if (!VerificadorNif.NifValido(nif)) {
                throw new ArgumentException(Msg.Nif_invalido);
            }
            Id = id;
            Versao = 0;
            Nome = nome;
            Nif = nif;
            Contactos = contactos ?? Enumerable.Empty<Contacto>();
            IdTipoFuncionario = idTipoFuncionario;
        }

        public Guid Id { get; private set; }
        public int Versao { get; private set; }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(Id != Guid.Empty);
            Contract.Invariant(!string.IsNullOrEmpty(Nome));
            Contract.Invariant(!string.IsNullOrEmpty(Nif));
            Contract.Invariant(IdTipoFuncionario > 0);
            Contract.Invariant(Contactos != null);
        }
    }
}