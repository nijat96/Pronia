using Microsoft.AspNetCore.Mvc;

namespace Pronia.Areas.ProniaAdminPanel.Controllers
{
    [Area("ProniaAdminPanel")]
    public class SinaqController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
