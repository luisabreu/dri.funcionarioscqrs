using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;

namespace Domain.Mensagens.Comandos {
    internal static class VerificadorNif {
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
            return Convert.ToInt32(Char.GetNumericValue(currentChar));
        }
    }
}