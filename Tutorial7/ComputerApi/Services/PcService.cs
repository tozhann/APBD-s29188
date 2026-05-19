using ComputerApi.Data;
using ComputerApi.DTOs;
using ComputerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ComputerApi.Services;

public class PcService : IPcService
{
    private readonly AppDbContext _db;

    public PcService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<PcListDto>> GetAllAsync()
    {
        return await _db.PCs
            .Select(p => new PcListDto
            {
                Id        = p.Id,
                Name      = p.Name,
                Weight    = p.Weight,
                Warranty  = p.Warranty,
                CreatedAt = p.CreatedAt,
                Stock     = p.Stock
            })
            .ToListAsync();
    }

    public async Task<PcComponentsDto?> GetComponentsAsync(int id)
    {
        var pc = await _db.PCs
            .Include(p => p.PCComponents)
                .ThenInclude(pc => pc.Component)
                    .ThenInclude(c => c.ComponentType)
            .Include(p => p.PCComponents)
                .ThenInclude(pc => pc.Component)
                    .ThenInclude(c => c.Manufacturer)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pc is null) return null;

        return new PcComponentsDto
        {
            PcId   = pc.Id,
            PcName = pc.Name,
            Components = pc.PCComponents.Select(pcc => new ComponentItemDto
            {
                Code          = pcc.ComponentCode.Trim(),
                Name          = pcc.Component.Name,
                Description   = pcc.Component.Description,
                Amount        = pcc.Amount,
                ComponentType = pcc.Component.ComponentType.Abbreviation,
                Manufacturer  = pcc.Component.Manufacturer.FullName
            }).ToList()
        };
    }

    public async Task<PcListDto> CreateAsync(CreatePcDto dto)
    {
        var pc = new PC
        {
            Name      = dto.Name,
            Weight    = dto.Weight,
            Warranty  = dto.Warranty,
            CreatedAt = dto.CreatedAt,
            Stock     = dto.Stock
        };

        _db.PCs.Add(pc);
        await _db.SaveChangesAsync();

        return new PcListDto
        {
            Id        = pc.Id,
            Name      = pc.Name,
            Weight    = pc.Weight,
            Warranty  = pc.Warranty,
            CreatedAt = pc.CreatedAt,
            Stock     = pc.Stock
        };
    }

    public async Task<PcListDto?> UpdateAsync(int id, UpdatePcDto dto)
    {
        var pc = await _db.PCs.FindAsync(id);
        if (pc is null) return null;

        pc.Name      = dto.Name;
        pc.Weight    = dto.Weight;
        pc.Warranty  = dto.Warranty;
        pc.CreatedAt = dto.CreatedAt;
        pc.Stock     = dto.Stock;

        await _db.SaveChangesAsync();

        return new PcListDto
        {
            Id        = pc.Id,
            Name      = pc.Name,
            Weight    = pc.Weight,
            Warranty  = pc.Warranty,
            CreatedAt = pc.CreatedAt,
            Stock     = pc.Stock
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var pc = await _db.PCs.FindAsync(id);
        if (pc is null) return false;

        _db.PCs.Remove(pc);
        await _db.SaveChangesAsync();
        return true;
    }
}
