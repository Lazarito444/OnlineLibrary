using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Infrastructure.Persistence.Contexts;
using OnlineLibrary.Presentation.Web.Middleware;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class BookController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly ValidateUserSession _validateUserSession;

    public BookController(AppDbContext dbContext, ValidateUserSession validateUserSession)
    {
        _dbContext = dbContext;
        _validateUserSession = validateUserSession;
    }

    public async Task<IActionResult> Index()
    {
        List<Book> books = await _dbContext.Set<Book>()
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .ToListAsync();
        return View(books);
    }

    public async Task<IActionResult> Create()
    {
        List<Author> authors = await _dbContext.Set<Author>().ToListAsync();
        List<Publisher> publishers = await _dbContext.Set<Publisher>().ToListAsync();

        List<SelectListItem> viewAuthors = new List<SelectListItem>();
        List<SelectListItem> viewPublishers = new List<SelectListItem>();
        
        foreach (Author author in authors)
        {
            viewAuthors.Add(new SelectListItem { Text = author.FullName, Value = author.Id.ToString() });
        }
        
        foreach (Publisher publisher in publishers)
        {
            viewPublishers.Add(new SelectListItem { Text = publisher.Name, Value = publisher.Id.ToString() });
        }

        ViewBag.ViewAuthors = viewAuthors;
        ViewBag.ViewPublishers = viewPublishers;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Book book)
    {
        await _dbContext.Set<Book>().AddAsync(book);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        Book currentBook = (await _dbContext.Set<Book>()
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .SingleOrDefaultAsync(b => b.Id == id))!;
        
        List<Author> authors = await _dbContext.Set<Author>().ToListAsync();
        List<Publisher> publishers = await _dbContext.Set<Publisher>().ToListAsync();

        List<SelectListItem> viewAuthors = new List<SelectListItem>();
        List<SelectListItem> viewPublishers = new List<SelectListItem>();
        
        foreach (Author author in authors)
        {
            viewAuthors.Add(new SelectListItem { Text = author.FullName, Value = author.Id.ToString() });
        }
        
        foreach (Publisher publisher in publishers)
        {
            viewPublishers.Add(new SelectListItem { Text = publisher.Name, Value = publisher.Id.ToString() });
        }

        ViewBag.ViewAuthors = viewAuthors;
        ViewBag.ViewPublishers = viewPublishers;
        
        return View(currentBook);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Book book)
    {
        Book currentBook = (await _dbContext.Set<Book>().FindAsync(book.Id))!;
        _dbContext.Set<Book>().Entry(currentBook).CurrentValues.SetValues(book);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        Book currentBook = (await _dbContext.Set<Book>()
            .Include(book => book.Author)
            .Include(book => book.Publisher)
            .SingleOrDefaultAsync(b => b.Id == id))!;
        return View(currentBook);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Book book)
    {
        Book currentBook = (await _dbContext.Set<Book>().FindAsync(book.Id))!;
        _dbContext.Set<Book>().Remove(currentBook);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}