using Microsoft.AspNetCore.Mvc;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class AuthorController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}