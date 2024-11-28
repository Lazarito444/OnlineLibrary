using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Infrastructure.Persistence.Contexts;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class CatalogController : Controller
{
    private readonly AppDbContext _dbContext;

    public CatalogController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        List<Book> books = await _dbContext.Set<Book>().ToListAsync();
        return View(books);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string? query)
    {
        if (String.IsNullOrWhiteSpace(query)) return await Index();
        List<Book> books = await _dbContext.Set<Book>().Where(book => book.Title.Contains(query)).ToListAsync();
        return View(books);
    }

    public async Task<IActionResult> ViewDetails(int id)
    {
        Book book = await _dbContext.Set<Book>()
            .Include(book => book.Publisher)
            .Include(book => book.Author)
            .FirstAsync(book => book.Id == id);

        return View(book);
    }

    public async Task<IActionResult> BorrowBook(int id)
    {
        return View();
    }
}