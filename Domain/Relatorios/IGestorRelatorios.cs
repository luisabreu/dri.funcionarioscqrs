using System;
using System.Collections.Generic;

namespace Domain.Relatorios {
    public interface IGestorRelatorios {
        IEnumerable<TipoFuncionario> ObtemTodosTiposFuncionarios();
        IEnumerable<ResumoFuncionario> Pesquisa(string nifOuNome);
        Funcionario Obtem(Guid id);
    }
}