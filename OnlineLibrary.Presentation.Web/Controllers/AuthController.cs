using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Core.Domain.Enums;
using OnlineLibrary.Core.Domain.Settings;
using OnlineLibrary.Infrastructure.Persistence.Contexts;
using OnlineLibrary.Presentation.Web.Extensions;
using OnlineLibrary.Presentation.Web.Middleware;
using OnlineLibrary.Presentation.Web.Services;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly ValidateUserSession _validateUserSession;
    private readonly EmailService _emailService;

    public AuthController(AppDbContext dbContext, ValidateUserSession validateUserSession, EmailService emailService)
    {
        _dbContext = dbContext;
        _validateUserSession = validateUserSession;
        _emailService = emailService;
    }

    public IActionResult Index()
    {
        if (_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        return View("Login");
    }
    
    public IActionResult Login()
    {
        if (_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        return View();
    }

    public IActionResult Register()
    {
        if (_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(User user)
    {
        if (_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        User? foundUser = await _dbContext.Set<User>().SingleOrDefaultAsync(u => u.Email == user.Email && u.Password == user.Password);

        if (foundUser == null) return View(user);

        HttpContext.Session.Set("LoggedUser", foundUser);
        
        return RedirectToRoute(new { Controller = "Book", Action = "Index" });
    }

    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        if (_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }

        User? userWithSameEmail = await _dbContext.Set<User>().FirstOrDefaultAsync(u => user.Email == u.Email);

        if (userWithSameEmail != null) return View(user);
        
        user.Role = Roles.User;
        await _dbContext.Set<User>().AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Welcome));
    }

    public IActionResult Welcome()
    {
        if (_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        return View();
    }
    
    public IActionResult LogOut()
    {
        if (!_validateUserSession.HasUser())
        {
            return RedirectToAction(nameof(Login));
        }
        
        User? sessionUser = HttpContext.Session.Get<User>("LoggedUser");

        if (sessionUser == null) return RedirectToAction(nameof(Login));
        
        HttpContext.Session.Remove("LoggedUser");

        return RedirectToAction(nameof(Login));
    }

    public IActionResult ForgotPassword()
    {
        if (_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        if (_validateUserSession.HasUser())
        {
            return RedirectToRoute(new { Controller = "Book", Action = "Index" });
        }
        
        User? user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Email == email);

        ResetToken? existingToken = await _dbContext.Set<ResetToken>().FirstOrDefaultAsync(rt => rt.User.Email == email);

        if (existingToken != null)
        {
            _emailService.SendEmail(new EmailContent
            {
                To = email,
                Subject = "Online Library | Password Reset",
                Message = $"<p>Hello! You've requested a password reset, <a href=\"http://{HttpContext.Request.Host}/Auth/ResetPassword?token={existingToken.Token}\">click here</a> to reset your password on our online library system</p>"
            });
        
            return RedirectToAction(nameof(Login));
        }
        if (user == null) return View();
        
        Guid token = Guid.NewGuid();
        await _dbContext.Set<ResetToken>().AddAsync(new ResetToken
        {
            UserId = user.Id,
            Token = token
        });
        await _dbContext.SaveChangesAsync();

        _emailService.SendEmail(new EmailContent
        {
            To = email,
            Subject = "Online Library | Password Reset",
            Message = $"<p>Hello! You've requested a password reset, <a href=\"http://{HttpContext.Request.Host}/Auth/ResetPassword?token={token}\">click here</a> to reset your password on our online library system</p>"
        });
        
        return RedirectToAction(nameof(Login));
    }

    public IActionResult ResetPassword(string token)
    {
        ViewData["token"] = token;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPasswordPost(string token, string newPassword)
    {
        ResetToken resetToken = _dbContext.Set<ResetToken>().FirstOrDefault(rt => rt.Token.ToString() == token)!;
        User user = (await _dbContext.Set<User>().FindAsync(resetToken.UserId))!;

        user.Password = newPassword;
        _dbContext.Set<ResetToken>().Remove(resetToken);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Login));
    }
}