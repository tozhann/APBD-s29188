namespace UserPanelApp.Models;

public class UserNote
{
    public int Id { get; set; }
    public int AppUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AppUser AppUser { get; set; } = null!;
}
