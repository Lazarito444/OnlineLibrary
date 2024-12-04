using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Infrastructure.Persistence.Contexts;
using OnlineLibrary.Presentation.Web.Extensions;
using OnlineLibrary.Presentation.Web.Middleware;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class CatalogController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly ValidateUserSession _validateUserSession;

    public CatalogController(AppDbContext dbContext, ValidateUserSession validateUserSession)
    {
        _dbContext = dbContext;
        _validateUserSession = validateUserSession;
    }

    public async Task<IActionResult> Index()
    {
        if (!_validateUserSession.HasClientUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        List<Book> books = await _dbContext.Set<Book>().ToListAsync();
        return View(books);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string? query)
    {
        if (!_validateUserSession.HasClientUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        if (String.IsNullOrWhiteSpace(query)) return await Index();
        List<Book> books = await _dbContext.Set<Book>().Where(book => book.Title.Contains(query)).ToListAsync();
        return View(books);
    }

    public async Task<IActionResult> ViewDetails(int id)
    {
        if (!_validateUserSession.HasClientUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        Book book = await _dbContext.Set<Book>()
            .Include(book => book.Publisher)
            .Include(book => book.Author)
            .FirstAsync(book => book.Id == id);

        return View(book);
    }

    public async Task<IActionResult> BorrowBook(int id)
    {
        if (!_validateUserSession.HasClientUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        Book book = await _dbContext.Set<Book>()
            .Include(book => book.Publisher)
            .Include(book => book.Author)
            .FirstAsync(book => book.Id == id);

        return View("BorrowBookConfirmation", book);
    }
    
    public async Task<IActionResult> BorrowBookPost(int id)
    {
        if (!_validateUserSession.HasClientUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }

        int userId = HttpContext.Session.Get<User>("LoggedUser")!.Id;
        BorrowedBook? borrowedBook = await _dbContext.Set<BorrowedBook>()
            .Where(bb => bb.BookId == id || bb.UserId == userId)
            .FirstOrDefaultAsync();

        if (borrowedBook == null)
        {
            Book book = await _dbContext.Set<Book>()
                .Include(book => book.Publisher)
                .Include(book => book.Author)
                .FirstAsync(book => book.Id == id);

            await _dbContext.Set<BorrowedBook>().AddAsync(new BorrowedBook
            {
                BookId = book.Id,
                UserId = userId,
                BorrowDate = DateTime.Now
            });
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            TempData[""] = "true";
        }
        

        
        return RedirectToAction(nameof(Index));
    }
}