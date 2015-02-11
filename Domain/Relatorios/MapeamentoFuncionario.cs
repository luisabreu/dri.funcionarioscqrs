using FluentNHibernate.Mapping;

namespace Domain.Relatorios {
    internal class MapeamentoFuncionario {
        public class MapeamentoFuncionarioDto : ClassMap<Funcionario> {
            public MapeamentoFuncionarioDto() {
                Table("Funcionarios");
                Not.LazyLoad();
                ReadOnly();

                Id(f => f.Id)
                    .GeneratedBy.Identity();
                Version(f => f.Versao);
                Map(f => f.Nome)
                    .Not.Nullable();
                Map(f => f.Nif)
                    .Not.Nullable();

                HasMany(f => f.Contactos)
                    .AsBag()
                    .Table("Contactos")
                    .KeyColumn("IdFuncionario")
                    .Component(c => {
                        c.Map(ct => ct.TipoContacto)
                            .CustomType<int>()
                            .CustomSqlType("integer");
                        c.Map(ct => ct.Valor);
                    })
                    .Cascade.None()
                    .Not.LazyLoad();

                References(f => f.TipoFuncionario, "IdTipoFuncionario")
                    .Cascade.None()
                    .Not.LazyLoad();
            }
        }
    }
}