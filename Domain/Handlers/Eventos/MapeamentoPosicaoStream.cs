using FluentNHibernate.Mapping;

namespace Domain.Handlers.Eventos {
    public class MapeamentoPosicaoStream {
        internal class MapeamentoFuncionario {
            public class MapeamentoFuncionarioDto : ClassMap<PosicaoStream> {
                public MapeamentoFuncionarioDto() {
                    Table("PosicaoStream");
                    Not.LazyLoad();
                    ReadOnly();

                    Id(f => f.IdPosicaoStream)
                        .GeneratedBy.Identity();
                    Version(f => f.Versao);
                    Map(f => f.PosicaoCommit)
                        .Not.Nullable();
                    Map(f => f.PosicaoPreparacao)
                        .Not.Nullable();
                }
            }
        }
    }
}