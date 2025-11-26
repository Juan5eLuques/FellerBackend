using FellerBackend.DTOs.Autos;
using FellerBackend.DTOs.Motos;

namespace FellerBackend.Services.Interfaces;

public interface IVehiculoService
{
    // Autos
    Task<List<AutoDto>> GetAllAutosAsync();
    Task<AutoDto?> GetAutoByIdAsync(int id);
    Task<AutoDto> CreateAutoAsync(CreateAutoDto dto);
    Task<AutoDto> UpdateAutoAsync(int id, UpdateAutoDto dto);
    Task<bool> DeleteAutoAsync(int id);

    // Motos
    Task<List<MotoDto>> GetAllMotosAsync();
    Task<MotoDto?> GetMotoByIdAsync(int id);
    Task<MotoDto> CreateMotoAsync(CreateMotoDto dto);
    Task<MotoDto> UpdateMotoAsync(int id, UpdateMotoDto dto);
    Task<bool> DeleteMotoAsync(int id);

    // Destacados
    Task<List<AutoDto>> GetAutosDestacadosAsync();
    Task<List<MotoDto>> GetMotosDestacadasAsync();
    Task<List<object>> GetVehiculosDestacadosAsync(); // Mixto: autos y motos
    Task<AutoDto> MarcarAutoComoDestacadoAsync(int id, int? orden = null);
    Task<MotoDto> MarcarMotoComoDestacadaAsync(int id, int? orden = null);
    Task<AutoDto> DesmarcarAutoDestacadoAsync(int id);
    Task<MotoDto> DesmarcarMotoDestacadaAsync(int id);

    // Común
    Task<bool> VehiculoExistsAsync(int id);
}
