using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Infrastructure.Persistence.Contexts;
using OnlineLibrary.Presentation.Web.Extensions;
using OnlineLibrary.Presentation.Web.Middleware;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class BorrowedBookController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly ValidateUserSession _validateUserSession;

    public BorrowedBookController(AppDbContext dbContext, ValidateUserSession validateUserSession)
    {
        _dbContext = dbContext;
        _validateUserSession = validateUserSession;
    }

    public async Task<IActionResult> Index()
    {
        if (!_validateUserSession.HasClientUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Index" });
        }
        
        int sessionUserId = HttpContext.Session.Get<User>("LoggedUser")!.Id;
        List<BorrowedBook> borrowedBooks = await _dbContext.Set<BorrowedBook>()
            .Include(bb => bb.Book)
            .ThenInclude(b => b.Author)
            .Where(bb => bb.UserId == sessionUserId)
            .OrderByDescending(bb => bb.BorrowDate)
            .ToListAsync();
        
        return View(borrowedBooks);
    }

    public async Task<IActionResult> ReturnBook(int id)
    {
        if (!_validateUserSession.HasClientUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Index" });
        }
        
        BorrowedBook borrowedBook = await _dbContext.Set<BorrowedBook>()
            .Include(bb => bb.Book)
            .ThenInclude(b => b.Author)
            .FirstAsync(bb => bb.Id == id);
        
        return View(borrowedBook);
    }

    [HttpPost]
    public async Task<IActionResult> ReturnBookPost(int id)
    {
        if (!_validateUserSession.HasClientUser())
        {
            return RedirectToRoute(new { Controller = "Auth", Action = "Index" });
        }
        
        BorrowedBook borrowedBook = (await _dbContext.Set<BorrowedBook>().FindAsync(id))!;
        borrowedBook.Returned = true;
        await _dbContext.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }
}