using FellerBackend.Services.Interfaces;

namespace FellerBackend.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> EnviarEmailAsync(string destinatario, string asunto, string mensaje)
    {
        // TODO: Implementar integración con servicio de email (SendGrid, AWS SES, etc.)
        // Por ahora es un placeholder que simula el envío
        
   _logger.LogInformation($"?? [EMAIL SIMULADO] Para: {destinatario} | Asunto: {asunto} | Mensaje: {mensaje}");
        
        // Simular delay de red
        await Task.Delay(100);
 
        return true;
    }
}
