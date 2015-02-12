using Domain.Relatorios;

namespace site.Models {
    public class DadosGerais {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Nif { get; set; }
        public TipoFuncionario TipoFuncionario { get; set; }
        public int Versao { get; set; }
    }
}