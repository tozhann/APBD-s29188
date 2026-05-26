using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserPanelApp.Data;

namespace UserPanelApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    // GET /Admin
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _db.AppUsers
            .AsNoTracking()
            .Select(u => new { u.Id, u.Email, u.Role, u.CreatedAt })
            .ToListAsync();

        return View(users);
    }
}
