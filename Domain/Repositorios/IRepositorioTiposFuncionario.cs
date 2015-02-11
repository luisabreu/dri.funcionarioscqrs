using System.Collections.Generic;
using Domain.Agregados.Entidades;

namespace Domain.Repositorios {
    public interface IRepositorioTiposFuncionario {
        IEnumerable<TipoFuncionario> ObtemTiposFuncionario();
    }
}