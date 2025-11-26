using FellerBackend.Services.Interfaces;

namespace FellerBackend.Services;

public class WhatsAppService : IWhatsAppService
{
    private readonly ILogger<WhatsAppService> _logger;

    public WhatsAppService(ILogger<WhatsAppService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> EnviarMensajeAsync(string telefono, string mensaje)
    {
      // TODO: Implementar integración con API de WhatsApp (Twilio, WhatsApp Business API, etc.)
        // Por ahora es un placeholder que simula el envío
        
   _logger.LogInformation($"?? [WHATSAPP SIMULADO] Enviando a {telefono}: {mensaje}");
     
        // Simular delay de red
  await Task.Delay(100);
        
        return true;
    }
}
