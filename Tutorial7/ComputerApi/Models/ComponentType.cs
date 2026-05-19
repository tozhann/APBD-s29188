namespace ComputerApi.Models;

public class ComponentType
{
    public int Id { get; set; }
    public string Abbreviation { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public ICollection<Component> Components { get; set; } = new List<Component>();
}
