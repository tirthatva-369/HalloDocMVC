using BusinessLogic.Interfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Project_HalloDoc.Models;
using System.Diagnostics;

namespace Project_HalloDoc.Controllers
{
    public class AspnetusersController : Controller
    {
        private readonly ILogger<AspnetusersController> _logger;
        private readonly ILoginInterface _loginService;
        private readonly IPatientInterface _patientService;
        private readonly IPatientInterface _familyService;
        private readonly IPatientInterface _conciergeService;
        private readonly IPatientInterface _businessService;

        public AspnetusersController(ILogger<AspnetusersController> logger, ILoginInterface loginService, IPatientInterface patientService, IPatientInterface familyService, IPatientInterface conciergeService, IPatientInterface businessService)
        {
            _logger = logger;
            _loginService = loginService;
            _patientService = patientService;
            _familyService = familyService;
            _conciergeService = conciergeService;
            _businessService = businessService;
        }

        [HttpPost]
        public IActionResult b2_registered_user(LoginModel loginModel)
        {
            if ((_loginService.EmailCheck(loginModel)) && (_loginService.PasswordCheck(loginModel)))
            {

                return RedirectToAction("b2c1_patient_dashboard", "Home");
            }

            else if (!(_loginService.EmailCheck(loginModel)))
            {
                TempData["Email"] = "Enter Valid Email";
                TempData["Password"] = "Enter Valid Password";
                return RedirectToAction("b2_registered_user", "Home");
            }

            else if (!(_loginService.PasswordCheck(loginModel)))
            {
                TempData["Password"] = "Enter Valid Password";
                return RedirectToAction("b2_registered_user", "Home");
            }

            else
            {
                return RedirectToAction("b2_registered_user", "Home");
            }
        }

        //public IActionResult b2c1_patient_dashboard()
        //{
        //    var infos = _patientService.GetMedicalHistory("1@g.c");
        //    var viewmodel = new MedicalHistoryList { medicalHistoriesList = infos };
        //    return View(viewmodel);
        //}

        [HttpPost]
        public IActionResult b1_submit_request_screen(PatientRequestModel patientRequestModel)
        {
            _patientService.AddPatientInfo(patientRequestModel);
            return RedirectToAction("b1_submit_request_screen", "Home");
        }

        public IActionResult b1_submit_request_screen(FamilyRequestModel familyRequestModel)
        {
            _familyService.AddFamilyRequest(familyRequestModel);
            return RedirectToAction("b1_submit_request_screen", "Home");
        }

        public IActionResult b1_submit_request_screen(ConciergeRequestModel conciergeRequestModel)
        {
            _familyService.AddConciergeRequest(conciergeRequestModel);
            return RedirectToAction("b1_submit_request_screen", "Home");
        }

        public IActionResult b1_submit_request_screen(BusinessRequestModel businessRequestModel)
        {
            _familyService.AddBusinessRequest(businessRequestModel);
            return RedirectToAction("b1_submit_request_screen", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            var emailExists = await _patientService.IsEmailExists(email);
            return Json(new { emailExists });
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}