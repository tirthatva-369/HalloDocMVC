using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Models;
using DataAccess.DataModels;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.Security.Cryptography;

namespace HalloDoc.mvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminInterface _adminService;
        private readonly INotyfService _notyf;

        public AdminController(ILogger<AdminController> logger, IAdminInterface adminService, INotyfService notyf)
        {
            _logger = logger;
            _adminService = adminService;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult admin_login()
        {
            return View();
        }

        public static string GenerateSHA256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hashEngine = SHA256.Create())
            {
                var hashedBytes = hashEngine.ComputeHash(bytes, 0, bytes.Length);
                var sb = new StringBuilder();
                foreach (var b in hashedBytes)
                {
                    var hex = b.ToString("x2");
                    sb.Append(hex);
                }
                return sb.ToString();
            }
        }
        public IActionResult AdminLogin(AdminLoginModel adminLoginModel)
        {
            if (ModelState.IsValid)
            {
                var aspnetuser = _adminService.GetAspnetuser(adminLoginModel.email);
                if (aspnetuser != null)
                {
                    adminLoginModel.password = GenerateSHA256(adminLoginModel.password);
                    if (aspnetuser.Passwordhash == adminLoginModel.password)
                    {
                        _notyf.Success("Logged in Successfully");
                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                    else
                    {
                        _notyf.Error("Password is incorrect");

                        return View();
                    }
                }
                _notyf.Error("Email is incorrect");
                return View();
            }
            else
            {
                return View(adminLoginModel);
            }
        }
        public IActionResult GetCount()
        {
            var statusCountModel = _adminService.GetStatusCount();
            return PartialView("_AllRequest", statusCountModel);
        }


        public IActionResult viewcase(int reqClientId)
        {
            var obj = _adminService.ViewCase(reqClientId);
            return View(obj);
        }

        public IActionResult viewnotes(int requestId)
        {
            var obj = _adminService.ViewNotes(requestId);
            return View(obj);
        }

        public IActionResult GetRequestsByStatus(int tabNo)
        {
            var list = _adminService.GetRequestsByStatus(tabNo);
            if (tabNo == 1)
            {
                return PartialView("_NewRequests", list);
            }
            else if (tabNo == 2)
            {
                return PartialView("_PendingRequests", list);
            }
            else if (tabNo == 3)
            {
                return PartialView("_ActiveRequests", list);
            }
            else if (tabNo == 4)
            {
                return PartialView("_ConcludeRequests", list);
            }
            else if (tabNo == 5)
            {
                return PartialView("_ToCloseRequests", list);
            }
            else if (tabNo == 6)
            {
                return PartialView("_UnpaidRequests", list);
            }
            return View();
        }

        [HttpPost]
        public IActionResult UpdateNotes(ViewNotesModel model)
        {
            bool isUpdated = _adminService.UpdateAdminNotes(model.AdminNotes, model.RequestId);
            if (isUpdated)
            {
                return RedirectToAction("viewnotes", "Admin", new { requestId = model.RequestId });
            }
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
            return View();
        }
    }
}