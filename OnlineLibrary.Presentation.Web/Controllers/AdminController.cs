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
        if (!_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        List<User> users = await _dbContext.Set<User>().ToListAsync();
        return View(users);
    }

    public IActionResult Create()
    {
        if (!_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user)
    {
        if (!_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        user.Role = Roles.Admin;
        await _dbContext.Set<User>().AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id)
    {
        if (!_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        User user = (await _dbContext.Set<User>().FindAsync(id))!;
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(User user)
    {
        if (!_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        user.Role = Roles.Admin;
        User userToUpdate = (await _dbContext.Set<User>().FindAsync(user.Id))!;
        _dbContext.Set<User>().Entry(userToUpdate).CurrentValues.SetValues(user);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        if (!_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        User user = (await _dbContext.Set<User>().FindAsync(id))!;
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(User user)
    {
        if (!_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        _dbContext.Set<User>().Remove(user);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}