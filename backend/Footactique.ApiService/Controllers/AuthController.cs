using Microsoft.AspNetCore.Mvc;

namespace Footactique.ApiService.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
