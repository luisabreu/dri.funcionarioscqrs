using Domain.Eventos;

namespace Domain.Mensagens {
    public interface IComando : IMensagem {
        int Versao { get; }
    }
}