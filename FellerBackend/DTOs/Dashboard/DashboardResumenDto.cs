namespace FellerBackend.DTOs.Dashboard;

public class DashboardResumenDto
{
    public int AutosPublicados { get; set; }
    public int MotosPublicadas { get; set; }
    public int TurnosDelDia { get; set; }
    public int UsuariosRegistrados { get; set; }
    public int TurnosPendientes { get; set; }
    public int TurnosEnProceso { get; set; }
}
