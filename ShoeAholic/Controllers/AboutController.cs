using Microsoft.AspNetCore.Mvc;

namespace ShoeAholic.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}