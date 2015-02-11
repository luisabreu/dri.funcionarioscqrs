using System;
using System.Collections.Generic;
using Domain.VO;

namespace Domain.Relatorios {
    public class Funcionario {
        public Guid Id { get; set; }
        public String Nome { get; set; }
        public String Nif { get; set; }
        public int Versao { get; set; }
        public TipoFuncionario TipoFuncionario { get; set; }
        public IEnumerable<Contacto> Contactos { get; set; }
    }
}