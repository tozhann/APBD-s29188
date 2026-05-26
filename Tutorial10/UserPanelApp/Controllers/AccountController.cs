using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserPanelApp.Data;
using UserPanelApp.Models;
using UserPanelApp.ViewModels;

namespace UserPanelApp.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;

    public AccountController(AppDbContext db)
    {
        _db = db;
    }

    // GET /Account/Register
    [HttpGet]
    public IActionResult Register() => View();

    // POST /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var emailTaken = await _db.AppUsers.AnyAsync(u => u.Email == model.Email);
        if (emailTaken)
        {
            ModelState.AddModelError("Email", "An account with this email already exists.");
            return View(model);
        }

        var user = new AppUser
        {
            Email        = model.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Role         = "User",
            CreatedAt    = DateTime.UtcNow
        };

        _db.AppUsers.Add(user);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Account created. Please log in.";
        return RedirectToAction("Login");
    }

    // GET /Account/Login
    [HttpGet]
    public IActionResult Login() => View();

    // POST /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Email == model.Email);

        // Generic error — do not reveal whether email or password was wrong
        if (user is null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid credentials.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email,           user.Email),
            new Claim(ClaimTypes.Role,            user.Role)
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Dashboard");
    }

    // POST /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
