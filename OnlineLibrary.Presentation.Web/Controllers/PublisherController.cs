using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Infrastructure.Persistence.Contexts;
using OnlineLibrary.Presentation.Web.Middleware;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class PublisherController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly ValidateUserSession _validateUserSession;

    public PublisherController(AppDbContext dbContext, ValidateUserSession validateUserSession)
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
        
        List<Publisher> publishers = await _dbContext.Set<Publisher>().ToListAsync();
        return View(publishers);
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
    public async Task<IActionResult> Create(Publisher publisher)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        await _dbContext.Set<Publisher>().AddAsync(publisher);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        Publisher? currentPublisher = await _dbContext.Set<Publisher>().FindAsync(id);

        if (currentPublisher == null) return RedirectToAction(nameof(Index));
        
        return View(currentPublisher);
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        Publisher? currentPublisher = await _dbContext.Set<Publisher>().FindAsync(id);

        if (currentPublisher == null) return RedirectToAction(nameof(Index));
        
        return View(currentPublisher);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Publisher publisher)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        _dbContext.Set<Publisher>().Remove(publisher);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Publisher publisher)
    {
        if (!_validateUserSession.HasAdminUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Login"});
        }
        
        Publisher currentPublisher = (await _dbContext.Set<Publisher>().FindAsync(publisher.Id))!;
        _dbContext.Set<Publisher>().Entry(currentPublisher).CurrentValues.SetValues(publisher);    
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
}