using ComputerApi.DTOs;

namespace ComputerApi.Services;

public interface IPcService
{
    Task<List<PcListDto>> GetAllAsync();
    Task<PcComponentsDto?> GetComponentsAsync(int id);
    Task<PcListDto> CreateAsync(CreatePcDto dto);
    Task<PcListDto?> UpdateAsync(int id, UpdatePcDto dto);
    Task<bool> DeleteAsync(int id);
}
