using DataAccess.DataContext;
using DataAccess.DataModels;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Project_HalloDoc.Models;
using System.Diagnostics;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Project_HalloDoc.Views.Home;
using AspNetCoreHero.ToastNotification.Abstractions;
using DataAccess.Enums;
using DataAccess.Enum;
using System.Drawing;

namespace HalloDoc.mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notyf;
        private readonly IAdminInterface _adminService;

        public HomeController(ILogger<HomeController> logger, IAdminInterface adminService, INotyfService notyfService)
        {
            _logger = logger;
            _notyf = notyfService;
            _adminService = adminService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ReviewAgreement(int reqId)
        {

            var status = _adminService.GetStatusForReviewAgreement(reqId);
            if (status == (int)StatusEnum.MDEnRoute)
            {
                TempData["ReviewStatus"] = "Review Agreement is Already Accepted !!";
                return RedirectToAction("AgreementStatus");
            }
            else if (status == (int)StatusEnum.CancelledByPatient)
            {
                TempData["ReviewStatus"] = "Review Agreement is Already Cancelled by patient !!";
                return RedirectToAction("AgreementStatus");
            }
            else
            {
                AgreementModel model = new()
                {
                    Reqid = reqId,
                };

                return View(model);
            }
        }

        [HttpPost]
        public IActionResult ReviewAgreement(AgreementModel agreementModal)
        {

            bool isSaved = _adminService.AgreeAgreement(agreementModal);
            if (isSaved)
            {
                _notyf.Success("Agreement Accepted");
                return RedirectToAction("Login", "Patient");
            }
            _notyf.Error("Something went wrong");
            return RedirectToAction("ReviewAgreement", new { reqId = agreementModal.Reqid });

        }

        public IActionResult AgreementStatus()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CancelAgreement(int reqId)
        {
            var model = _adminService.CancelAgreement(reqId);
            return PartialView("_cancelagreement", model);
        }

        [HttpPost]
        public IActionResult CancelAgreement(AgreementModel model)
        {
            bool isCancelled = _adminService.SubmitCancelAgreement(model);
            if (isCancelled)
            {
                _notyf.Success("Agreement Cancelled");
                return RedirectToAction("Login", "Patient");
            }
            _notyf.Error("Something went wrong");
            return RedirectToAction("ReviewAgreement", new { reqId = model.Reqid });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}