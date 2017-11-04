using Microsoft.AspNetCore.Mvc;

namespace SchoolChallenge.Client.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Error() => View();
    }
}