using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_HalloDoc.Models;
using System.Diagnostics;

namespace Project_HalloDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult b1_submit_request_screen()
        {
            return View();
        }

        public IActionResult b1c1_patient_request()
        {
            return View();
        }

        public IActionResult b1c2_family_friend_request()
        {
            return View();
        }

        public IActionResult b1c3_concierge_request()
        {
            return View();
        }

        public IActionResult b1c4_business_partner_request()
        {
            return View();
        }

        public IActionResult b2_registered_user()
        {
            return View();
        }

        public IActionResult b2c_forgot_password()
        {
            return View();
        }

        public IActionResult b2c1_patient_dashboard()
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