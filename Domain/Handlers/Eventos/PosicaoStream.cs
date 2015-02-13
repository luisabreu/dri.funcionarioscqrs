namespace Domain.Handlers.Eventos {
    public class PosicaoStream {
        public int IdPosicaoStream { get; set; }
        public long PosicaoCommit { get; set; }
        public long PosicaoPreparacao { get; set; }
    }

}