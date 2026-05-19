namespace ComputerApi.Models;

public class Component
{
    public string Code { get; set; } = string.Empty;  // char(10) PK
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ComponentManufacturersId { get; set; }
    public int ComponentTypesId { get; set; }

    public ComponentManufacturer Manufacturer { get; set; } = null!;
    public ComponentType ComponentType { get; set; } = null!;
    public ICollection<PCComponent> PCComponents { get; set; } = new List<PCComponent>();
}
