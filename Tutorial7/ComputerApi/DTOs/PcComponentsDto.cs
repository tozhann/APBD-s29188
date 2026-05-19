namespace ComputerApi.DTOs;

public class PcComponentsDto
{
    public int PcId { get; set; }
    public string PcName { get; set; } = string.Empty;
    public List<ComponentItemDto> Components { get; set; } = new();
}

public class ComponentItemDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Amount { get; set; }
    public string ComponentType { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
}
