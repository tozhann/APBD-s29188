namespace ComputerApi.Models;

public class PC
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public float Weight { get; set; }
    public int Warranty { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Stock { get; set; }

    public ICollection<PCComponent> PCComponents { get; set; } = new List<PCComponent>();
}
