using FluentNHibernate.Mapping;

namespace Domain.Handlers.Eventos {
    public class MapeamentoPosicaoStream {
        internal class MapeamentoFuncionario {
            public class MapeamentoFuncionarioDto : ClassMap<PosicaoStream> {
                public MapeamentoFuncionarioDto() {
                    Table("PosicaoStream");
                    Not.LazyLoad();

                    Id(f => f.IdPosicaoStream)
                        .GeneratedBy.Assigned();
                   
                    Map(f => f.PosicaoCommit)
                        .Not.Nullable();
                    Map(f => f.PosicaoPreparacao)
                        .Not.Nullable();
                }
            }
        }
    }
}