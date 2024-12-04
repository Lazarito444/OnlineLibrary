using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Infrastructure.Persistence.Contexts;
using OnlineLibrary.Presentation.Web.Extensions;
using OnlineLibrary.Presentation.Web.Middleware;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class ProfileController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly ValidateUserSession _validateUserSession;

    public ProfileController(AppDbContext dbContext, ValidateUserSession validateUserSession)
    {
        _dbContext = dbContext;
        _validateUserSession = validateUserSession;
    }

    public IActionResult Index()
    {
        if (!_validateUserSession.HasClientUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Index" });
        }

        User user = HttpContext.Session.Get<User>("LoggedUser")!;
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser(User user)
    {
        if (!_validateUserSession.HasClientUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Index" });
        }

        User userToUpdate = (await _dbContext.Set<User>().FindAsync(user.Id))!;

        if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.FullName) ||
            user.DateOfBirth == default)
        {
            TempData[""] = "true";
            return View("Index", userToUpdate);
        }

        if (user.Email != userToUpdate.Email || 
            await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Email == user.Email) == null)
        {
            TempData["emailTaken"] = "true";
            return View("Index", userToUpdate);
        }
        user.Role = userToUpdate.Role;

        if (user.Password == null)
        {
            user.Password = userToUpdate.Password;
        }
        _dbContext.Set<User>().Entry(userToUpdate).CurrentValues.SetValues(user);
        await _dbContext.SaveChangesAsync();
        HttpContext.Session.Set("LoggedUser", user);
        return RedirectToAction(nameof(Index));
    }
}