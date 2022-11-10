using Microsoft.AspNetCore.Mvc;
using simplemeet.Data;
using simplemeet.Models;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            if (!(User!.Identity!.IsAuthenticated))
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Topics = _context.Topic!
                            .Include(t => t.Users!);
            createUserIfFirstLogin(User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value);
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private void createUserIfFirstLogin(string Email)
        {
            var _profile = (_context.User!).FirstOrDefault(m => m.EmailAddress == Email)!;
            if (_profile == null)
            {

                User NewProfile = new User();
                NewProfile.EmailAddress = User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value!;
                NewProfile.ProfileImage = "default.png";
                NewProfile.Name = (User.Identity!).Name!;
                _context.Add(NewProfile);
                _context.SaveChanges();
            }
        }
    }
}