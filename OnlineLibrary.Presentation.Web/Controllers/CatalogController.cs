using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Core.Domain.Settings;
using OnlineLibrary.Infrastructure.Persistence.Contexts;
using OnlineLibrary.Presentation.Web.Extensions;
using OnlineLibrary.Presentation.Web.Middleware;
using OnlineLibrary.Presentation.Web.Services;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class CatalogController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly ValidateUserSession _validateUserSession;
    private readonly EmailService _emailService;

    public CatalogController(AppDbContext dbContext, ValidateUserSession validateUserSession, EmailService emailService)
    {
        _dbContext = dbContext;
        _validateUserSession = validateUserSession;
        _emailService = emailService;
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

        User user = HttpContext.Session.Get<User>("LoggedUser")!;
        int userId = user.Id;
        BorrowedBook? borrowedBook = await _dbContext.Set<BorrowedBook>()
            .Where(bb => (bb.BookId == id || bb.UserId == userId) && !bb.Returned)
            .FirstOrDefaultAsync();

        if (borrowedBook == null)
        {
            BorrowedBook? alreadyBookedByUser = await _dbContext.Set<BorrowedBook>().FirstOrDefaultAsync(bb => bb.BookId == id && bb.UserId == userId);

            if (alreadyBookedByUser != null)
            {
                _dbContext.Set<BorrowedBook>().Remove(alreadyBookedByUser);
            }
            
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
            
            _emailService.SendEmail(new EmailContent
            {
                To = user.Email,
                Subject = "Borrowed Book | Online Library",
                Message = $"Hey! You've borrowed the following book in our online library platform: {book.Title} by {book.Author.FullName}."
            });
        }
        else
        {
            TempData[""] = "true";
        }
        

        
        return RedirectToAction(nameof(Index));
    }
}