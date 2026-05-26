# User Panel – Tutorial 10

ASP.NET Core MVC application with registration, login, private dashboard, notes, and an admin panel.

## How to Run

1. Start SQL Server (local or Docker).
2. Update the connection string in `appsettings.json` if needed.
3. Run the application – migrations are applied automatically on startup:
   ```bash
   dotnet run
   ```
4. Open `https://localhost:{port}/Account/Login` in your browser.

## Test Users

| Email | Password | Role |
|---|---|---|
| admin@example.com | Admin@1234 | Admin |

To create a regular user, use the `/Account/Register` page.

## Logging in as Admin

Navigate to `/Account/Login` and use:
- **Email:** admin@example.com
- **Password:** Admin@1234

## Where Password Hashing is Configured

`Controllers/AccountController.cs`
- **Register:** `BCrypt.Net.BCrypt.HashPassword(model.Password)` – hashes before saving
- **Login:** `BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash)` – verifies without revealing the hash

## Where Authentication is Configured

`Program.cs` – `AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(...)`

After login, a `ClaimsPrincipal` is created and signed in via `HttpContext.SignInAsync`.

## Protected Actions

| Action | Protection |
|---|---|
| `DashboardController` (entire controller) | `[Authorize]` – any logged-in user |
| `AdminController` (entire controller) | `[Authorize(Roles = "Admin")]` – admins only |

---

## Security Questions

**Why must passwords not be stored as plain text?**
If the database is leaked, every user's password is immediately exposed. Hashed passwords require an attacker to crack each one individually.

**Why is raw SHA-256 not a good choice for passwords?**
SHA-256 is a fast general-purpose hash. Attackers can compute billions of SHA-256 hashes per second using GPUs, making brute-force and rainbow-table attacks practical. Password hashing algorithms like BCrypt are intentionally slow and include a work factor.

**Why do we use salt?**
Salt is a random value added to the password before hashing. It ensures that two users with the same password get different hashes, and it defeats precomputed rainbow-table attacks.

**What is the difference between salt and pepper?**
Salt is stored alongside the hash in the database and is unique per user. Pepper is a secret value stored outside the database (e.g. in environment config) and is the same for all users. Pepper adds an extra layer: even if the database is stolen, the attacker also needs the pepper.

**What is the difference between authentication and authorization?**
Authentication answers "Who are you?" – it verifies identity (login). Authorization answers "What are you allowed to do?" – it checks permissions (roles, claims).

**Why is hiding a link in a view not enough as security?**
A hidden link only changes the UI. The underlying URL is still accessible to anyone who types it directly. Authorization must be enforced on the server side with `[Authorize]`.

**Why can a "there is no such user" login message be a problem?**
It reveals to an attacker which emails are registered, enabling user enumeration. A generic message like "Invalid credentials." gives no information about whether the email or password was wrong.
