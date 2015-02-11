using System;

namespace Domain.Relatorios {
    public class ResumoFuncionario {
        public Guid Id { get; set; }
        public String Nome { get; set; }
        public String Nif { get; set; }
        public String TipoFuncionario { get; set; }
    }
}