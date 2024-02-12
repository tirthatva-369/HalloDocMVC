using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess.DataContext;
using DataAccess.DataModels;
using Project_HalloDoc.Models;
using System.Diagnostics;

namespace Project_HalloDoc.Controllers
{
    public class AspnetusersController : Controller
    {
        private readonly ILogger<AspnetusersController> _logger;
        private readonly ILoginInterface _loginService;
        //private readonly IPatientService _patientService;


        public AspnetusersController(ILogger<AspnetusersController> logger, ILoginInterface loginService)
        {
            _logger = logger;
            _loginService = loginService;
            //_patientService = patientService;
        }

        [HttpPost]
        public IActionResult b1_submit_request_screen(LoginModel loginModel)

        {
            if ((_loginService.EmailCheck(loginModel)) && (_loginService.PasswordCheck(loginModel)))
            {
                return RedirectToAction("b1c1_patient_request", "Home");
            }

            else if (!(_loginService.EmailCheck(loginModel)))
            {
                TempData["Email"] = "Enter Valid Email";
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}