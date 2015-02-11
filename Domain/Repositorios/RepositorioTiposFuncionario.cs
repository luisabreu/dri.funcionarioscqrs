using System.Collections.Generic;
using Domain.Agregados.Entidades;

namespace Domain.Repositorios {
    public class RepositorioTiposFuncionario : IRepositorioTiposFuncionario {
        public IEnumerable<TipoFuncionario> ObtemTiposFuncionario() {
            return new[] {new TipoFuncionario(1, "Docente"), new TipoFuncionario(2, "Normal")};
        }
    }
}