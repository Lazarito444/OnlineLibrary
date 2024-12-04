using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Infrastructure.Persistence.Contexts;
using OnlineLibrary.Presentation.Web.Middleware;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class AuthorController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly ValidateUserSession _validateUserSession;

    public AuthorController(AppDbContext dbContext, ValidateUserSession validateUserSession)
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
        
        List<Author> authors = await _dbContext.Set<Author>().ToListAsync();
        return View(authors);
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
    public async Task<IActionResult> Create(Author author)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        if (string.IsNullOrWhiteSpace(author.FullName))
        {
            TempData[""] = "true";
            return View();
        }
        
        _dbContext.Set<Author>().Add(author);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        Author author = (await _dbContext.Set<Author>().FindAsync(id))!;
        return View(author);
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        Author author = (await _dbContext.Set<Author>().FindAsync(id))!;
        return View(author);
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(Author author)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        _dbContext.Set<Author>().Remove(author);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Author author)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        Author currentAuthor = (await _dbContext.Set<Author>().FindAsync(author.Id))!;
        
        if (string.IsNullOrWhiteSpace(author.FullName))
        {
            TempData[""] = "true";
            return View(currentAuthor);
        }
        
        _dbContext.Set<Author>().Entry(currentAuthor).CurrentValues.SetValues(author);    
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}