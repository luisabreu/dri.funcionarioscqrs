using Domain.Eventos;

namespace Domain.Mensagens {
    public interface IEvento : IMensagem {
        int Versao { get; set; }
    }
}