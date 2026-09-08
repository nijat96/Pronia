using Microsoft.AspNetCore.Mvc;

namespace Pronia.Areas.ProniaAdminPanel.Controllers
{
    [Area("ProniaAdminPanel")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
