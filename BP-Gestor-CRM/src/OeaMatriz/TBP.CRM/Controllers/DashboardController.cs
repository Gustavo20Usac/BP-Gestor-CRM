using Microsoft.AspNetCore.Mvc;

namespace TBP.CRM.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
