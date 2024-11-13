using Microsoft.AspNetCore.Mvc;

namespace OnlineLibrary.Presentation.Web.Controllers;

public class BookController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}