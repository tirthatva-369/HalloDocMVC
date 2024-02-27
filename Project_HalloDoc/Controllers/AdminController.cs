using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Models;
using DataAccess.DataModels;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.mvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminInterface _adminService;


        public AdminController(ILogger<AdminController> logger, IAdminInterface adminService)
        {
            _logger = logger;
            _adminService = adminService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult admin_login()
        {
            return View();
        }

        public IActionResult viewcase()
        {
            return View();
        }

        public IActionResult admin_resetpassword()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult admin_login(AdminLoginModel adminLoginModel)
        {
            if (ModelState.IsValid)
            {

                return RedirectToAction("admin_dashboard", "Admin");
            }
            return View(adminLoginModel);

        }
        public IActionResult admin_dashboard()
        {
            var list = _adminService.GetRequestsByStatus();
            return View(list);
        }
    }
}