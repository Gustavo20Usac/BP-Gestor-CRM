using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OeaMatriz.WebMvc.Controllers;

/// <summary>
/// Home controller providing the dashboard for authenticated users.
/// </summary>
[Authorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}