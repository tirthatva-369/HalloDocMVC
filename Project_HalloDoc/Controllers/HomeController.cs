using DataAccess.DataContext;
using DataAccess.DataModels;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Project_HalloDoc.Models;
using System.Diagnostics;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Project_HalloDoc.Views.Home;

namespace HalloDoc.mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        //private readonly AdminService _adminService;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db/*, AdminService adminService*/)
        {
            _logger = logger;
            _db = db;
            //_adminService = adminService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        //public IActionResult ReviewAgreement(int reqId)
        //{
        //    AgreementModal am = new AgreementModal();
        //    am.Reqid = reqId;
        //    return View(am);
        //}
        //public IActionResult AgreeAgreement(AgreementModal agreementModal)
        //{
        //    var model = _adminService.IAgreeAgreement(agreementModal);
        //    return RedirectToAction("AdminDashboard", "Admin", model);
        //}

        //public IActionResult CancelAgreement(AgreementModal agreementModal)
        //{
        //    var model = _adminService.ICancelAgreement(agreementModal);
        //    return PartialView("_cancelagreement", model);
        //}

        //[HttpPost]
        //public IActionResult CancelAgreementSubmit(int ReqClientid, string Description)
        //{
        //    AgreementModal model = new()
        //    {
        //        ReqClientId = ReqClientid,
        //        Reason = Description,
        //    };
        //    var obj = _adminService.SubmitCancelAgreement(model);
        //    return RedirectToAction("AdminDashboard", "Admin", obj);
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}