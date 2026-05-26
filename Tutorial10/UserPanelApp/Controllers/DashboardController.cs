using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserPanelApp.Data;
using UserPanelApp.Models;
using UserPanelApp.ViewModels;

namespace UserPanelApp.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /Dashboard
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        var user   = await _db.AppUsers
            .Include(u => u.Notes)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var vm = new DashboardViewModel
        {
            Email = user!.Email,
            Notes = user.Notes.OrderByDescending(n => n.CreatedAt).ToList()
        };

        return View(vm);
    }

    // GET /Dashboard/AddNote
    [HttpGet]
    public IActionResult AddNote() => View(new AddNoteViewModel());

    // POST /Dashboard/AddNote
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddNote(AddNoteViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var note = new UserNote
        {
            AppUserId = GetUserId(),
            Title     = model.Title,
            Content   = model.Content,
            CreatedAt = DateTime.UtcNow
        };

        _db.UserNotes.Add(note);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Note added.";
        return RedirectToAction("Index");
    }
}
