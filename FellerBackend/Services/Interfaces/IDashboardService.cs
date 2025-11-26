using FellerBackend.DTOs.Dashboard;

namespace FellerBackend.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardResumenDto> GetResumenAsync();
}
