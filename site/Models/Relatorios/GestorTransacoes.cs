using System.Configuration;

namespace site.Models.Relatorios {
    public class GestorTransacoes : Domain.Relatorios.GestorTransacoes {
        public override string ObtemCnnString() {
            return ConfigurationManager.ConnectionStrings["bd"].ConnectionString;
        }
    }
}