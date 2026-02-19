using Microsoft.AspNetCore.Mvc;

namespace SportStore.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
