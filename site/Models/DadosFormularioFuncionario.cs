using System.Collections.Generic;
using Domain.Relatorios;

namespace site.Models {
    public class DadosFormularioFuncionario {
        public Funcionario Funcionario { get; set; }
        public IEnumerable<TipoFuncionario> TiposFuncionario { get; set; }
        public bool Novo { get; set; }
    }
}