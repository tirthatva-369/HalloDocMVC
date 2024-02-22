using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.DataContext;
using DataAccess.DataModels;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_HalloDoc.Models;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

namespace Project_HalloDoc.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoginInterface _loginService;
        private readonly IPatientInterface _patientService;
        private readonly ApplicationDbContext _db;
        private readonly IPatientInterface _familyService;
        private readonly IPatientInterface _conciergeService;
        private readonly IPatientInterface _businessService;

        public HomeController(ILogger<HomeController> logger, ILoginInterface loginService, IPatientInterface patientService, ApplicationDbContext db, IPatientInterface familyService, IPatientInterface conciergeService, IPatientInterface businessService)
        {
            _logger = logger;
            _loginService = loginService;
            _patientService = patientService;
            _db = db;
            _familyService = familyService;
            _conciergeService = conciergeService;
            _businessService = businessService;
        }

        [HttpPost]
        public IActionResult b2_registered_user(LoginModel loginModel)
        {
            var user = _loginService.Login(loginModel);
            if ((_loginService.EmailCheck(loginModel)) && (_loginService.PasswordCheck(loginModel)))
            {
                return RedirectToAction("b2c1_patient_dashboard", "Home", user);
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

        public IActionResult b2c1d2_for_someone_request()
        {
            return View();
        }

        public IActionResult b2c1d1_for_me_request()
        {
            return View();
        }

        [HttpPost]
        public IActionResult b1c1_patient_request(PatientRequestModel patientRequestModel)
        {
            if (ModelState.IsValid)
            {
                _patientService.AddPatientInfo(patientRequestModel);
                return RedirectToAction("b1_submit_request_screen", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult b1c2_family_friend_request(FamilyRequestModel familyRequestModel)
        {
            if (ModelState.IsValid)
            {
                _familyService.AddFamilyRequest(familyRequestModel);
                return RedirectToAction("b1_submit_request_screen", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult b1c3_concierge_request(ConciergeRequestModel conciergeRequestModel)
        {
            if (ModelState.IsValid)
            {
                _conciergeService.AddConciergeRequest(conciergeRequestModel);
                return RedirectToAction("b1_submit_request_screen", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult b1c4_business_partner_request(BusinessRequestModel businessRequestModel)
        {
            if (ModelState.IsValid)
            {
                _businessService.AddBusinessRequest(businessRequestModel);
                return RedirectToAction("b1_submit_request_screen", "Home");
            }
            else
            {
                return View();
            }
        }

        public IActionResult b2c1_patient_dashboard(User user)
        {
            var infos = _patientService.GetMedicalHistory(user);
            return View(infos);
        }


        public IActionResult GetDcoumentsById(int requestId)
        {
            var list = _patientService.GetAllDocById(requestId);
            return PartialView("_DocumentList", list.ToList());
        }

        public IActionResult Edit(MedicalHistory medicalHistory)
        {

            var existingUser = _db.Users.FirstOrDefault(x => x.Email == medicalHistory.Email);
            existingUser.Firstname = medicalHistory.FirstName;
            existingUser.Lastname = medicalHistory.LastName;
            //existingUser.dob = medicalHistory.DateOfBirth;
            existingUser.Email = medicalHistory.Email;
            //existingUser. = medicalHistory.ContactType;
            existingUser.Mobile = medicalHistory.PhoneNumber;
            existingUser.Street = medicalHistory.Street;
            existingUser.City = medicalHistory.City;
            existingUser.State = medicalHistory.State;
            existingUser.Zip = medicalHistory.ZipCode;

            _db.Users.Update(existingUser);
            _db.SaveChanges();
            return RedirectToAction("b2c1_patient_dashboard", "Home", existingUser);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult PatientResetPasswordEmail(Aspnetuser user)
        {
            string Id = _db.Aspnetusers.FirstOrDefault(x => x.Email == user.Email).Id;
            string resetPasswordUrl = GenerateResetPasswordUrl(Id);
            SendEmail(user.Email, "Reset Your Password", $"Hello, reset your password using this link: {resetPasswordUrl}");

            return RedirectToAction("b2_registered_user", "Home");
        }

        private string GenerateResetPasswordUrl(string userId)
        {
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string resetPasswordPath = Url.Action("SetPassword", "Home", new { id = userId });
            return baseUrl + resetPasswordPath;
        }

        private Task SendEmail(string email, string subject, string message)
        {
            var mail = "tatva.dotnet.tirthpatel@outlook.com";
            var password = "Prabodham@369";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };
            return client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
        }

        // Handle the reset password URL in the same controller or in a separate one
        public IActionResult SetPassword(string id)
        {
            var aspuser = _db.Aspnetusers.FirstOrDefault(x => x.Id == id);
            return View(aspuser);
        }

        [HttpPost]
        public IActionResult SetPassword(Aspnetuser aspnetuser)
        {
            var aspnuser = _db.Aspnetusers.FirstOrDefault(x => x.Id == aspnetuser.Id);
            aspnuser.Passwordhash = aspnetuser.Passwordhash;
            _db.Aspnetusers.Update(aspnuser);
            _db.SaveChanges();
            return RedirectToAction("b2_registered_user");
        }
    }
}