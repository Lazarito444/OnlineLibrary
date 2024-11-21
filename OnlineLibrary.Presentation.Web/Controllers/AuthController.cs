using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Core.Domain.Enums;
using OnlineLibrary.Infrastructure.Persistence.Contexts;
using OnlineLibrary.Presentation.Web.Extensions;
using OnlineLibrary.Presentation.Web.Middleware;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly ValidateUserSession _validateUserSession;

    public AuthController(AppDbContext dbContext, ValidateUserSession validateUserSession)
    {
        _dbContext = dbContext;
        _validateUserSession = validateUserSession;
    }

    public IActionResult Index()
    {
        if (_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        return View("Login");
    }
    
    public IActionResult Login()
    {
        if (_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        return View();
    }

    public IActionResult Register()
    {
        if (_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(User user)
    {
        if (_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        User? foundUser = await _dbContext.Set<User>().SingleOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);

        if (foundUser == null) return View(user);

        HttpContext.Session.Set("LoggedUser", foundUser);
        
        return RedirectToRoute(new { Controller = "Book", Action = "Index" });
    }

    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        if (_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }

        User? userWithSameEmail = await _dbContext.Set<User>().FirstOrDefaultAsync(u => user.Email == u.Email);

        if (userWithSameEmail != null) return View(user);
        
        user.Role = Roles.User;
        await _dbContext.Set<User>().AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Login));
    }
    
    public IActionResult LogOut()
    {
        if (!_validateUserSession.HasUser())
        {
            return RedirectToAction(nameof(Login));
        }
        
        User? sessionUser = HttpContext.Session.Get<User>("LoggedUser");

        if (sessionUser == null) return RedirectToAction(nameof(Login));
        
        HttpContext.Session.Remove("LoggedUser");

        return RedirectToAction(nameof(Login));
    }
}