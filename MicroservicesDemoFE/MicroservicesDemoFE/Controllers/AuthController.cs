using Microsoft.AspNetCore.Mvc;

namespace MicroservicesDemoFE.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
