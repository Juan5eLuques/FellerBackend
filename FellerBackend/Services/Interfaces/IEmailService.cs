namespace FellerBackend.Services.Interfaces;

public interface IEmailService
{
    Task<bool> EnviarEmailAsync(string destinatario, string asunto, string mensaje);
}
