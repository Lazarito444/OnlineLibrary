using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Core.Domain.Settings;
using OnlineLibrary.Infrastructure.Persistence.Contexts;
using OnlineLibrary.Presentation.Web.Middleware;
using OnlineLibrary.Presentation.Web.Services;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class BooksLentController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly ValidateUserSession _validateUserSession;
    private readonly EmailService _emailService;

    public BooksLentController(AppDbContext dbContext, ValidateUserSession validateUserSession, EmailService emailService)
    {
        _dbContext = dbContext;
        _validateUserSession = validateUserSession;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index()
    {
        List<BorrowedBook> borrowedBooks = await _dbContext.Set<BorrowedBook>()
            .Include(bb => bb.Book)
            .ThenInclude(b => b.Author)
            .Include(bb => bb.User)
            .ToListAsync();
        
        return View(borrowedBooks);
    }

    [HttpPost]
    public async Task<IActionResult> SendEmail(int id)
    {
        BorrowedBook borrowedBook = await _dbContext.Set<BorrowedBook>()
            .Include(bb => bb.User)
            .Include(bb => bb.Book)
            .FirstAsync(bb => bb.Id == id);
        
        _emailService.SendEmail(new EmailContent
        {
            To = borrowedBook.User.Email,
            Subject = "Kind reminder to return book | Online Library",
            Message = $"Dear {borrowedBook.User.FullName}, this is a kind reminder to return the book '{borrowedBook.Book.Title}' that you borrowed on {borrowedBook.BorrowDate:d} on our Online Library Platform."
        });

        TempData[""] = $"Email sent to {borrowedBook.User.Email} successfully.";
        return RedirectToAction(nameof(Index));
    }
}