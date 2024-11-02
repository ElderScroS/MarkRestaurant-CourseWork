using MarkRestaurant.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MarkRestaurant.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) { _logger = logger; }

        public IActionResult Index() => View();
        public IActionResult Menu() => View();
        public IActionResult About() => View();
        public IActionResult BookTable() => View();

        public IActionResult Error(string title, string description)
        {
            var errorViewModel = new ErrorViewModel(title, description);

            return View(errorViewModel);
        }
        public IActionResult Error(string title, string description, string aspController, string aspAction)
        {
            var errorViewModel = new ErrorViewModel(title, description, aspController, aspAction);

            return View(errorViewModel);
        }
    }
}