using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace subscription_watch.Controllers
{
    public class DashboardController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
