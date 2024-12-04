using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Core.Domain.Enums;
using OnlineLibrary.Infrastructure.Persistence.Contexts;
using OnlineLibrary.Presentation.Web.Middleware;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class AdminController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly ValidateUserSession _validateUserSession;

    public AdminController(AppDbContext dbContext, ValidateUserSession validateUserSession)
    {
        _dbContext = dbContext;
        _validateUserSession = validateUserSession;
    }
    
    public async Task<IActionResult> Index()
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        List<User> users = await _dbContext.Set<User>().ToListAsync();
        return View(users);
    }

    public IActionResult Create()
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.FullName) ||
            user.DateOfBirth == null || string.IsNullOrWhiteSpace(user.Password))
        {
            TempData[""] = "true";
            return View();
        }
        
        if (await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Email == user.Email) != null)
        {
            TempData["emailTaken"] = "true";
            return View();
        }
        
        user.Role = Roles.Admin;
        await _dbContext.Set<User>().AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        User user = (await _dbContext.Set<User>().FindAsync(id))!;
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(User user)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        User userToUpdate = (await _dbContext.Set<User>().FindAsync(user.Id))!;

        if (string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.FullName) ||
            user.DateOfBirth == null)
        {
            TempData[""] = "true";
            return View(userToUpdate);
        }

        if (user.Email != userToUpdate.Email && await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Email == user.Email) == null)
        {
            TempData["emailTaken"] = "true";
            return View(userToUpdate);
        }
        user.Role = Roles.Admin;

        if (user.Password == null)
        {
            user.Password = userToUpdate.Password;
        }
        
        _dbContext.Set<User>().Entry(userToUpdate).CurrentValues.SetValues(user);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        User user = (await _dbContext.Set<User>().FindAsync(id))!;
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(User user)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        _dbContext.Set<User>().Remove(user);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}