using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Domain.VO {
    public class Contacto {
        private static readonly Regex _verificadorNumTel = new Regex(@"^\d{9}$");
        private static readonly Regex _verificadorNumExtensao = new Regex(@"^\d{4}$");

        public Contacto(TipoContacto tipoContacto, string valor) {
            Contract.Requires(!string.IsNullOrEmpty(valor));
            TipoContacto = tipoContacto;
            Valor = valor;
        }

        public TipoContacto TipoContacto { get; private set; }
        public String Valor { get; private set; }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant() {
            Contract.Invariant(!string.IsNullOrEmpty(Valor));
        }

        public static Contacto CriaTelefone(string numero) {
            Contract.Requires(numero != null, Msg.Contacto_incorreto);
            if (!_verificadorNumTel.IsMatch(numero)) {
                throw new InvalidOperationException(Msg.Contacto_incorreto);
            }
            return new Contacto(TipoContacto.Telefone, numero);
        }

        private static bool EmailValido(string valor) {
            try {
                new MailAddress(valor);
                return true;
            }
            catch {
                return false;
            }
        }

        public static Contacto CriaEmail(string mail) {
            Contract.Requires(mail != null, Msg.Contacto_incorreto);
            if (!EmailValido(mail)) {
                throw new InvalidOperationException(Msg.Contacto_incorreto);
            }
            return new Contacto(TipoContacto.Email, mail);
        }

        public static Contacto CriaExtensao(string ext) {
            Contract.Requires(ext != null, Msg.Contacto_incorreto);
            if (!_verificadorNumExtensao.IsMatch(ext)) {
                throw new InvalidOperationException(Msg.Contacto_incorreto);
            }
            return new Contacto(TipoContacto.Extensao, ext);
        }

        public static Contacto Parses(string contacto) {
            if (new Regex(@"^\d{9}$").IsMatch(contacto)) {
                return CriaTelefone(contacto);
            }
            if (new Regex(@"^\d{4}$").IsMatch(contacto)) {
                return CriaExtensao(contacto);
            }
            try {
                new MailAddress(contacto);
                return CriaEmail(contacto);
            }
            catch (Exception) {
                return null;
            }
        }
    }
}