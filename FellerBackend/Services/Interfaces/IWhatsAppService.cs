namespace FellerBackend.Services.Interfaces;

public interface IWhatsAppService
{
    Task<bool> EnviarMensajeAsync(string telefono, string mensaje);
}
