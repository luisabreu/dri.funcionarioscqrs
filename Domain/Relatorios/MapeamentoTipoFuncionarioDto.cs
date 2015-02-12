using FluentNHibernate.Mapping;

namespace Domain.Relatorios {
    internal class MapeamentoTipoFuncionarioDto : ClassMap<TipoFuncionario> {
        public MapeamentoTipoFuncionarioDto()
        {
            Table("TipoFuncionario");
            Not.LazyLoad();
            ReadOnly();

            Id(f => f.IdTipoFuncionario)
                .GeneratedBy.Identity();
            Map(f => f.Descricao)
                .Not.Nullable();
        }
    }
}