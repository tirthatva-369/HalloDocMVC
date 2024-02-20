﻿using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.DataContext;
using DataAccess.DataModels;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_HalloDoc.Models;
using System.Diagnostics;
using System.Globalization;

namespace Project_HalloDoc.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoginInterface _loginService;
        private readonly IPatientInterface _patientService;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ILoginInterface loginService, IPatientInterface patientService, ApplicationDbContext db)
        {
            _logger = logger;
            _loginService = loginService;
            _patientService = patientService;
            _db = db;

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


    }

}