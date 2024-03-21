﻿using BusinessLogic.Interfaces;
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
using DataAccess.DataContext;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        private readonly ApplicationDbContext _db;


        public AdminController(ILogger<AdminController> logger, IAdminInterface adminService, INotyfService notyf, IJwtService jwtService, ApplicationDbContext db)
        {
            _logger = logger;
            _adminService = adminService;
            _notyf = notyf;
            _jwtService = jwtService;
            _db = db;
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
            else if (tabNo == 0)
            {
                return Json(list);
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

            var message = string.Join(", ", selectedFiles);

            _notyf.Success("Send Mail Successfully");

            var mail = "tatva.dotnet.tirthpatel@outlook.com";
            var password = "Prabodham@369";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("tatva.dotnet.tirthpatel@outlook.com"),
                Subject = "Document List",
                IsBodyHtml = true,

            };
            foreach (var item in selectedFiles)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UploadedFiles");
                path = Path.Combine(path, item);
                Attachment attachment = new Attachment(path);
                mailMessage.Attachments.Add(attachment);
            }
            //RequestClient requestClient = _requestClientRepository.GetRequestClientByRequestId(requestId);
            //mailMessage.To.Add(requestClient.Email);
            mailMessage.To.Add("tirthpatel7321@gmail.com");

            client.SendMailAsync(mailMessage);

            return RedirectToAction("ViewUploads", "Admin", new { reqId = rid });
        }

        public Task SendEmail(string email, string subject, string message)
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
            return PartialView("_Transfer", model);
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
            return PartialView("_Clearcase");
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
            var model = _adminService.ShowCloseCase(reqId);
            return View(model);
        }

        [HttpPost]
        public IActionResult CloseCase(CloseCaseModel model)
        {
            bool isSaved = _adminService.SaveCloseCase(model);
            if (isSaved)
            {
                _notyf.Success("Saved");
            }
            else
            {
                _notyf.Error("Failed");
            }
            return RedirectToAction("CloseCase", new { ReqId = model.reqid });
        }
        public IActionResult SubmitCloseCase(int ReqId)
        {
            bool isClosed = _adminService.SubmitCloseCase(ReqId);
            if (isClosed)
            {
                _notyf.Success("Closed Successfully");
                return RedirectToAction("AdminDashboard");
            }
            else
            {
                _notyf.Error("Failed to close");
                return RedirectToAction("CloseCase", new { ReqId = ReqId });
            }
        }


        [HttpGet]
        public IActionResult SendAgreement(int reqId, int reqType)
        {
            var model = _adminService.SendAgreementCase(reqId);
            model.reqType = reqType;
            return PartialView("_SendAgreement", model);
        }

        //public Task SendEmail(string email, string subject, string message)
        //{
        //    var mail = "tatva.dotnet.tirthpatel@outlook.com";
        //    var password = "Prabodham@369";

        //    var client = new SmtpClient("smtp.office365.com", 587)
        //    {
        //        EnableSsl = true,
        //        Credentials = new NetworkCredential(mail, password)
        //    };

        //    return client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
        //}

        private string GenerateReviewAgreementUrl(string userId)
        {
            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            string resetPasswordPath = Url.Action("ReviewAgreement", "Home", new { id = userId });
            return baseUrl + resetPasswordPath;
        }

        [HttpPost]
        public IActionResult SendAgreement(SendAgreementModel model)
        {
            try
            {
                string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                string reviewPathLink = baseUrl + Url.Action("ReviewAgreement", "Home", new { reqId = model.Reqid });

                SendEmail(model.Email, "Review Agreement", $"Hello, Review the agreement properly: {reviewPathLink}");
                _notyf.Success("Agreement Sent");

            }
            catch (Exception ex)
            {
                _notyf.Error("Failed to sent");
            }
            return RedirectToAction("AdminDashboard");
        }

        [HttpGet]
        public IActionResult Encounter(int ReqId)
        {
            var model = _adminService.EncounterForm(ReqId);
            return View(model);
        }

        [HttpPost]
        public IActionResult Encounter(EncounterFormModel model)
        {
            bool isSaved = _adminService.SubmitEncounterForm(model);
            if (isSaved)
            {
                _notyf.Success("Saved!!");
            }
            else
            {
                _notyf.Error("Failed");
            }
            return RedirectToAction("Encounter", new { ReqId = model.reqid });
        }

        [HttpPost]
        public string ExportReq(List<AdminDashTableModel> reqList)
        {
            StringBuilder stringbuild = new StringBuilder();

            string header = "\"No\"," + "\"Name\"," + "\"DateOfBirth\"," + "\"Requestor\"," +
                "\"RequestDate\"," + "\"Phone\"," + "\"Notes\"," + "\"Address\","
                 + "\"Physician\"," + "\"DateOfService\"," + "\"Region\"," +
                "\"Status\"," + "\"RequestTypeId\"," + "\"OtherPhone\"," + "\"Email\"," + "\"RequestId\"," + Environment.NewLine + Environment.NewLine;

            stringbuild.Append(header);
            int count = 1;

            foreach (var item in reqList)
            {
                string content = $"\"{count}\"," + $"\"{item.firstName}\"," + $"\"{item.intDate}\"," + $"\"{item.requestorFname}\"," +
                    $"\"{item.intDate}\"," + $"\"{item.mobileNo}\"," + $"\"{item.notes}\"," + $"\"{item.street}\"," +
                    $"\"{item.lastName}\"," + $"\"{item.intDate}\"," + $"\"{item.street}\"," +
                    $"\"{item.status}\"," + $"\"{item.requestTypeId}\"," + $"\"{item.mobileNo}\"," + $"\"{item.firstName}\"," + $"\"{item.reqId}\"," + Environment.NewLine;

                count++;
                stringbuild.Append(content);
            }

            string finaldata = stringbuild.ToString();

            return finaldata;
            //return Json(new { message = finaldata });
        }

        [HttpGet]
        public IActionResult SendLink()
        {
            return PartialView("_SendLink");
        }
        [HttpPost]
        public IActionResult SendLink(SendLinkModel model)
        {
            bool isSend = false;
            try
            {
                string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                string reviewPathLink = baseUrl + Url.Action("RequestScreen", "Patient");

                SendEmail(model.email, "Create Patient Request", $"Hello, please create patient request from this link: {reviewPathLink}");
                _notyf.Success("Link Sent");
                isSend = true;
            }
            catch (Exception ex)
            {
                _notyf.Error("Failed to sent");
            }
            return Json(new { isSend = isSend });

        }

        [HttpGet]
        public IActionResult ShowMyProfile()
        {
            var request = HttpContext.Request;
            var token = request.Cookies["jwt"];
            if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                return Json("ok");
            }
            var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);

            var model = _adminService.MyProfile(emailClaim.Value);
            return PartialView("_MyProfile", model);
        }

        [HttpGet]
        public IActionResult CreateRequest()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VerifyState(string stateMain)
        {
            if (stateMain == null || stateMain.Trim() == null)
            {
                return Json(new { isSend = false });
            }
            var isSend = _adminService.VerifyState(stateMain);
            return Json(new { isSend = isSend });
        }


        [HttpPost]
        public IActionResult CreateRequest(CreateRequestModel model)
        {
            var request = HttpContext.Request;
            var token = request.Cookies["jwt"];
            if (token == null || !_jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                _notyf.Error("Token Expired");
                return View(model);
            }
            var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            var isSaved = _adminService.CreateRequest(model, emailClaim.Value);
            if (isSaved)
            {
                _notyf.Success("Request Created");
                return RedirectToAction("AdminDshboard");
            }
            else
            {
                _notyf.Error("Failed to Create");
                return View(model);
            }
        }

        public IActionResult RequestSupport()
        {
            return PartialView("_RequestSupport");
        }
    }
}