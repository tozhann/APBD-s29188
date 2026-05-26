using System.ComponentModel.DataAnnotations;
using UserPanelApp.Models;

namespace UserPanelApp.ViewModels;

public class DashboardViewModel
{
    public string Email { get; set; } = string.Empty;
    public List<UserNote> Notes { get; set; } = new();
}

public class AddNoteViewModel
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required.")]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}
