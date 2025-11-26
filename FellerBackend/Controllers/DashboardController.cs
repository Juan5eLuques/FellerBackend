using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FellerBackend.DTOs.Dashboard;
using FellerBackend.Helpers;
using FellerBackend.Services.Interfaces;

namespace FellerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("resumen")]
    public async Task<IActionResult> GetResumen()
    {
        try
        {
            var resumen = await _dashboardService.GetResumenAsync();
            return Ok(ResponseWrapper<DashboardResumenDto>.SuccessResponse(resumen));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseWrapper<object>.ErrorResponse("Error interno del servidor", new List<string> { ex.Message }));
        }
    }
}
