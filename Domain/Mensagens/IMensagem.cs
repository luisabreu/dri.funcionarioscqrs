using System;

namespace Domain.Eventos {
    public interface IMensagem {
        Guid IdAgregado { get; }
    }
}