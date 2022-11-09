using Microsoft.AspNetCore.Mvc;
using simplemeet.Data;
using simplemeet.Models;
using System.Diagnostics;

namespace simplemeet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly simplemeetContext _context;

        public HomeController(ILogger<HomeController> logger, simplemeetContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            //if (!(User!.Identity!.IsAuthenticated))
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}