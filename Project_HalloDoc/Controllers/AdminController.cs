using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Models;
using DataAccess.DataModels;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using AspNetCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using Project_HalloDoc.Auth;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Text.Json.Nodes;
//using Org.BouncyCastle.Crypto.Macs;
//using Org.BouncyCastle.Asn1.Ocsp;

namespace HalloDoc.mvc.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminInterface _adminService;
        private readonly INotyfService _notyf;
        private readonly IJwtService _jwtService;

        public AdminController(ILogger<AdminController> logger, IAdminInterface adminService, INotyfService notyf, IJwtService jwtService)
        {
            _logger = logger;
            _adminService = adminService;
            _notyf = notyf;
            _jwtService = jwtService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AdminLogin(AdminLoginModel adminLoginModel)
        {
            if (ModelState.IsValid)
            {
                var aspnetuser = _adminService.GetAspnetuser(adminLoginModel.email);
                if (aspnetuser != null)
                {
                    if (aspnetuser.Passwordhash == adminLoginModel.password)
                    {
                        var jwtToken = _jwtService.GetJwtToken(aspnetuser);
                        Response.Cookies.Append("jwt", jwtToken);

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

        [CustomAuthorize("Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("AdminLogin");
        }

        public IActionResult GetCount()
        {
            var statusCountModel = _adminService.GetStatusCount();
            return PartialView("_AllRequest", statusCountModel);
        }


        public IActionResult ViewCase(int Requestclientid, int RequestTypeId, int ReqId)
        {
            var obj = _adminService.ViewCase(Requestclientid, RequestTypeId, ReqId);

            return View(obj);
        }

        public IActionResult ViewNote(int ReqId)
        {
            HttpContext.Session.SetInt32("RNId", ReqId);
            var data = _adminService.ViewNotes(ReqId);
            return View(data);
        }

        public IActionResult GetRequestsByStatus(int tabNo)
        {
            var list = _adminService.GetRequestsByStatus(tabNo);
            if (tabNo == 1)
            {
                return PartialView("_NewRequest", list);
            }
            else if (tabNo == 2)
            {
                return PartialView("_PendingRequest", list);
            }
            else if (tabNo == 3)
            {
                return PartialView("_ActiveRequest", list);
            }
            else if (tabNo == 4)
            {
                return PartialView("_ConcludeRequest", list);
            }
            else if (tabNo == 5)
            {
                return PartialView("_ToCloseRequest", list);
            }
            else if (tabNo == 6)
            {
                return PartialView("_UnpaidRequest", list);
            }
            return View();
        }

        [HttpPost]
        public IActionResult UpdateNotes(ViewNotesModel model)
        {
            int? reqId = HttpContext.Session.GetInt32("RNId");
            bool isUpdated = _adminService.UpdateAdminNotes(model.AdminNotes, (int)reqId);
            if (isUpdated)
            {
                _notyf.Success("Saved Changes !!");
                return RedirectToAction("ViewNote", "Admin", new { ReqId = reqId });

            }
            return RedirectToAction("ViewNote", "Admin");
        }

        public IActionResult admin_resetpassword()
        {
            return View();
        }

        public IActionResult CancelCase(int reqId)
        {
            HttpContext.Session.SetInt32("CancelReqId", reqId);
            var model = _adminService.CancelCase(reqId);
            return PartialView("_CancelCase", model);
        }

        [HttpPost]
        public IActionResult SubmitCancelCase(int casetag, string notes, int reqId)
        {
            CancelCaseModel cancelCaseModel = new()
            {
                casetag = casetag,
                notes = notes,
                reqId = reqId
            };
            bool isCancelled = _adminService.SubmitCancelCase(cancelCaseModel);
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult Order(int reqId)
        {
            var order = _adminService.FetchProfession();
            order.ReqId = reqId;
            return View(order);
        }

        [HttpPost]
        public IActionResult orders(Order order)
        {
            bool isSend = _adminService.SendOrder(order);
            return Json(new { isSend = isSend });
        }

        //[HttpPost]
        //public IActionResult OrderDetails(Order order, int requestId)
        //{
        //    order.ReqId = requestId;
        //    _adminService.SendOrderDetails(order);
        //    return RedirectToAction("AdminDashboard", "Admin");
        //}

        [HttpGet]
        public JsonArray FetchBusiness(int proffesionId)
        {
            var result = _adminService.FetchVendors(proffesionId);
            return result;
        }

        [HttpGet]
        public Healthprofessional VendorDetails(int selectedValue)
        {
            var result = _adminService.VendorDetails(selectedValue);
            return result;
        }

        public IActionResult AssignCase(int reqId)
        {
            HttpContext.Session.SetInt32("AssignReqId", reqId);
            var model = _adminService.AssignCase(reqId);
            return PartialView("_AssignCase", model);
        }

        public IActionResult SubmitAssignCase(AssignCaseModel assignCaseModel)
        {
            assignCaseModel.ReqId = HttpContext.Session.GetInt32("AssignReqId");
            bool isAssigned = _adminService.SubmitAssignCase(assignCaseModel);
            if (isAssigned)
            {
                _notyf.Success("Assigned successfully");
                return RedirectToAction("AdminDashboard", "Admin");
            }
            return View();
        }

        public IActionResult GetPhysician(int selectRegion)
        {
            List<Physician> physicianlist = _adminService.GetPhysicianByRegion(selectRegion);
            return Json(new { physicianlist });
        }

        public IActionResult BlockCase(int reqId)
        {
            HttpContext.Session.SetInt32("BlockReqId", reqId);
            var model = _adminService.BlockCase(reqId);
            return PartialView("_BlockCase", model);
        }

        [HttpPost]
        public IActionResult SubmitBlockCase(BlockCaseModel blockCaseModel)
        {
            blockCaseModel.ReqId = HttpContext.Session.GetInt32("BlockReqId");
            bool isBlocked = _adminService.SubmitBlockCase(blockCaseModel);
            if (isBlocked)
            {
                _notyf.Success("Blocked Successfully");
                return RedirectToAction("AdminDashboard", "Admin");
            }
            _notyf.Error("BlockCase Failed");
            return RedirectToAction("AdminDashboard", "Admin");
        }

        public IActionResult DeleteFileById(int id)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            bool isDeleted = _adminService.DeleteFileById(id);
            if (isDeleted)
            {
                return RedirectToAction("viewuploads", "Admin", new { reqId = rid });
            }
            else
            {
                _notyf.Error("SomeThing Went Wrong");
                return RedirectToAction("viewuploads", "Admin", new { reqId = rid });
            }
        }

        public IActionResult DeleteAllFiles(List<string> selectedFiles)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            bool isDeleted = _adminService.DeleteAllFiles(selectedFiles, rid);
            if (isDeleted)
            {
                _notyf.Success("Deleted Successfully");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
            _notyf.Error("SomeThing Went Wrong");
            return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });

        }

        public IActionResult SendAllFiles(List<string> selectedFiles)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");

            SendEmail("yashvariya23@gmail.com", "Documents", selectedFiles);
            _notyf.Success("Send Mail Successfully");
            return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
        }

        private Task SendEmail(string email, string subject, List<string> filenames)
        {
            var mail = "tatva.dotnet.tirthpatel@outlook.com";
            var password = "Prabodham@369";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };
            MailMessage mailMessage = new MailMessage();
            for (var i = 0; i < filenames.Count; i++)
            {
                string pathname = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", filenames[i]);
                Attachment attachment = new Attachment(pathname);
                mailMessage.Attachments.Add(attachment);
            }
            mailMessage.To.Add(email);
            mailMessage.From = new MailAddress(mail);

            mailMessage.Subject = subject;

            return client.SendMailAsync(mailMessage);
        }

        public IActionResult ViewUploads(int reqId)
        {
            HttpContext.Session.SetInt32("rid", reqId);
            var model = _adminService.GetAllDocById(reqId);
            return View(model);
        }

        [HttpPost]
        public IActionResult UploadFiles(ViewUploadModel model)
        {
            var rid = (int)HttpContext.Session.GetInt32("rid");
            if (model.uploadedFiles == null)
            {
                _notyf.Error("First Upload Files");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
            bool isUploaded = _adminService.UploadFiles(model.uploadedFiles, rid);
            if (isUploaded)
            {
                _notyf.Success("Uploaded Successfully");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
            else
            {
                _notyf.Error("Upload Failed");
                return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
            }
        }

        [HttpGet]
        public IActionResult TransferCase(int reqId)
        {
            var model = _adminService.AssignCase(reqId);
            model.ReqId = reqId;
            return PartialView("_transfer", model);
        }

        [HttpPost]
        public IActionResult SubmitTransferCase(AssignCaseModel transferCaseModel)
        {
            bool isTransferred = _adminService.SubmitAssignCase(transferCaseModel);
            return Json(new { isTransferred = isTransferred });
        }

        [HttpGet]
        public IActionResult ClearCase(int reqId)
        {
            ViewBag.ClearCaseId = reqId;
            return PartialView("_clearcase");
        }

        [HttpPost]
        public IActionResult SubmitClearCase(int reqId)
        {
            bool isClear = _adminService.ClearCase(reqId);
            if (isClear)
            {
                _notyf.Success("Cleared Successfully");
                return RedirectToAction("AdminDashboard");
            }
            _notyf.Error("Failed");
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult CloseCase(int reqId)
        {
            var model = _adminService.GetAllDocById(reqId);
            return View(model);
        }

        [HttpGet]
        public IActionResult SendAgreement(int reqId, int reqType)
        {
            var model = _adminService.SendAgreementCase(reqId);
            model.reqType = reqType;
            return PartialView("_sendagreement", model);
        }

        [HttpPost]
        public IActionResult SendAgreement(string email)
        {
            try
            {
                string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                string reviewPathLink = baseUrl + Url.Action("ReviewAgreement", "Home");

                //SendEmail(email, "Review Agreement", $"Hello, Review the agreement properly: {reviewPathLink}");
                return Json(new { isSend = true });

            }
            catch (Exception ex)
            {
                return Json(new { isSend = false });
            }
        }
    }
}