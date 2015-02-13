using FluentNHibernate.Mapping;

namespace Domain.Relatorios {
    internal class MapeamentoFuncionario {
        public class MapeamentoFuncionarioDto : ClassMap<Funcionario> {
            public MapeamentoFuncionarioDto() {
                Table("Funcionarios");
                Not.LazyLoad();

                Id(f => f.Id)
                    .GeneratedBy.Assigned();
             
                Map(f => f.Nome)
                    .Not.Nullable();
                Map(f => f.Nif)
                    .Not.Nullable();
                Map(f => f.Versao)
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