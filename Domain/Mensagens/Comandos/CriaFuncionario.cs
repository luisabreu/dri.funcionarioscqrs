using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
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

        private static class VerificadorNif {
            private static readonly Regex _baseNifReg = new Regex("^\\d{9}$");

            public static Boolean NifValido(String nif) {
                Contract.Requires(!String.IsNullOrEmpty(nif));

                if (!_baseNifReg.IsMatch(nif)) {
                    return false;
                }

                if (!FirstNumberIsCorrect(nif)) return false;
                if (nif == null || nif.Length != 9) {
                    return false;
                }
                return CalculateCheckDigit(nif) == GetIntFromChar(nif[8]);
            }

            private static Boolean FirstNumberIsCorrect(String nif) {
                var validFirstChars = new[] {'1', '2', '5', '6', '7', '8', '9'};
                return validFirstChars.Contains(nif[0]);
            }

            [ContractVerification(false)]
            private static int CalculateCheckDigit(String nif) {
                var checkDigit = 0;
                for (var i = 0; i < 8; i++) {
                    checkDigit += GetIntFromChar(nif[i])*(9 - i);
                }

                checkDigit = 11 - (checkDigit%11);
                if (checkDigit >= 10) {
                    checkDigit = 0;
                }
                return checkDigit;
            }

            private static Int32 GetIntFromChar(char currentChar) {
                return Convert.ToInt32(char.GetNumericValue(currentChar));
            }
        }
    }
}