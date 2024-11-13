using Microsoft.AspNetCore.Mvc;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class AuthController : Controller
{
    public IActionResult Index()
    {
        return View("Login");
    }
    
    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }
}