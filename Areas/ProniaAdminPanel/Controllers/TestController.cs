using Microsoft.AspNetCore.Mvc;

namespace Pronia.Areas.ProniaAdminPanel.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
