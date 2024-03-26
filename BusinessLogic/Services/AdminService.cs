using BusinessLogic.Interfaces;
using DataAccess.DataContext;
using DataAccess.DataModels;
using DataAccess.Enum;
using DataAccess.Enums;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Text.Json.Nodes;
//using static Org.BouncyCastle.Math.EC.ECCurve;

namespace BusinessLogic.Services
{
    public class AdminService : IAdminInterface
    {
        private readonly ApplicationDbContext _db;
        private readonly IJwtService _jwtService;

        public AdminService(ApplicationDbContext db, IJwtService jwtService)
        {
            _db = db;
            _jwtService = jwtService;

        }

        //public Aspnetuser GetAspnetuser(string email)
        //{
        //    var aspNetUser = _db.Aspnetusers.Include(x => x.Aspnetuserroles).FirstOrDefault(x => x.Email == email);
        //    return aspNetUser;
        //}

        public LoginResponseViewModel AdminLogin(AdminLoginModel model)
        {
            var user = _db.Aspnetusers.Where(u => u.Email == model.email).FirstOrDefault();

            if (user == null)
                return new LoginResponseViewModel() { Status = ResponseStatus.Failed, Message = "User Not Found" };
            if (user.Passwordhash == null)
                return new LoginResponseViewModel() { Status = ResponseStatus.Failed, Message = "There is no Password with this Account" };
            if (user.Passwordhash == model.password)
            {
                var jwtToken = _jwtService.GetJwtToken(user);

                return new LoginResponseViewModel() { Status = ResponseStatus.Success, Token = jwtToken };
            }

            return new LoginResponseViewModel() { Status = ResponseStatus.Failed, Message = "Password does not match" };
        }

        public DashboardModel GetRequestsByStatus(int tabNo, int CurrentPage)
        {
            var query = from r in _db.Requests
                        join rc in _db.Requestclients on r.Requestid equals rc.Requestid
                        select new AdminDashTableModel
                        {
                            email = r.Email,
                            reqId = r.Requestid,
                            regionId = rc.Regionid,
                            firstName = rc.Firstname,
                            lastName = rc.Lastname,
                            intDate = rc.Intdate,
                            intYear = rc.Intyear,
                            strMonth = rc.Strmonth,
                            reqintDate = r.Createddate.Day,
                            reqintYear = r.Createddate.Year,
                            reqstrMonth = r.Createddate.ToString("MMM"),
                            requestorFname = r.Firstname,
                            requestorLname = r.Lastname,
                            createdDate = r.Createddate,
                            mobileNo = rc.Phonenumber,
                            city = rc.City,
                            state = rc.State,
                            street = rc.Street,
                            zipCode = rc.Zipcode,
                            requestTypeId = r.Requesttypeid,
                            status = r.Status,
                            reqClientId = rc.Requestclientid,
                            notes = rc.Notes,
                        };

            if (tabNo == 1)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Unassigned);
            }

            else if (tabNo == 2)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Accepted);
            }
            else if (tabNo == 3)
            {

                query = query.Where(x => x.status == (int)StatusEnum.MDEnRoute || x.status == (int)StatusEnum.MDOnSite);
            }
            else if (tabNo == 4)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Conclude);
            }
            else if (tabNo == 5)
            {

                query = query.Where(x => (x.status == (int)StatusEnum.Cancelled || x.status == (int)StatusEnum.CancelledByPatient) || x.status == (int)StatusEnum.Closed);
            }
            else if (tabNo == 6)
            {

                query = query.Where(x => x.status == (int)StatusEnum.Unpaid);
            }
            var result = query.ToList();
            DashboardModel dashboardModel = new DashboardModel();
            dashboardModel.adminDashTableList = result;
            dashboardModel.regionList = _db.Regions.ToList();
            var page = query.ToList();
            int count = result.Count();
            int TotalPage = (int)Math.Ceiling(count / (double)5);
            result = result.Skip((CurrentPage - 1) * 5).Take(5).ToList();

            dashboardModel.adminDashTableList = result;
            dashboardModel.TotalPage = TotalPage;
            dashboardModel.CurrentPage = CurrentPage;
            return dashboardModel;
        }
        public DashboardModel GetRequestByRegion(int regionId, int tabNo)
        {
            DashboardModel model = new DashboardModel();
            model = GetRequestsByStatus(tabNo, 1);
            model.adminDashTableList = model.adminDashTableList.Where(x => x.regionId == regionId).ToList();
            return model;
        }

        public StatusCountModel GetStatusCount()
        {
            var requestsWithClients = _db.Requests
     .Join(_db.Requestclients,
         r => r.Requestid,
         rc => rc.Requestid,
         (r, rc) => new { Request = r, RequestClient = rc })
     .ToList();

            StatusCountModel statusCount = new StatusCountModel
            {
                NewCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Unassigned),
                PendingCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Accepted),
                ActiveCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.MDEnRoute || x.Request.Status == (int)StatusEnum.MDOnSite),
                ConcludeCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Conclude),
                ToCloseCount = requestsWithClients.Count(x => (x.Request.Status == (int)StatusEnum.Cancelled || x.Request.Status == (int)StatusEnum.CancelledByPatient) || x.Request.Status == (int)StatusEnum.Closed),
                UnpaidCount = requestsWithClients.Count(x => x.Request.Status == (int)StatusEnum.Unpaid)
            };
            return statusCount;
        }

        public ViewCaseViewModel ViewCase(int reqClientId, int RequestTypeId, int ReqId)
        {
            var data = _db.Requestclients.FirstOrDefault(x => x.Requestclientid == reqClientId);
            var requestData = _db.Requests.FirstOrDefault(x => x.Requestid == data.Requestid);
            var userData = _db.Users.FirstOrDefault(x => x.Userid == requestData.Userid);
            var regionData = _db.Regions.FirstOrDefault(x => x.Regionid == data.Regionid);
            var reqtypeData = _db.Requesttypes.FirstOrDefault(x => x.Requesttypeid == requestData.Requesttypeid);

            var viewdata = new ViewCaseViewModel
            {
                RequestId = ReqId,
                Requestclientid = reqClientId,
                Firstname = data.Firstname,
                Lastname = data.Lastname,
                ConfirmationNumber = requestData.Confirmationnumber,
                Notes = data.Notes,
                Address = data.Street + ", " + data.City + ", " + data.State + ", " + data.Zipcode,
                Email = data.Email,
                Phonenumber = data.Phonenumber,
                City = data.City,
                State = data.State,
                RequestTypeId = RequestTypeId,
                Street = data.Street,
                Zipcode = data.Zipcode,
                RegionName = regionData.Name,
                Requesttype = reqtypeData.Name,
                //DateOfBirth = new DateTime(Convert.ToInt32(userData.Intyear), DateTime.ParseExact(userData.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(userData.Intdate)),
                //IntYear = userData.Intyear,
                //IntDate = userData.Intdate,
                //StrMonth = userData.Strmonth,

            };
            return viewdata;
        }

        //public ViewNotesModel ViewNotes(int requestId)
        //{
        //    var requestNotes = _db.Requestnotes.Where(x => x.Requestid == requestId).FirstOrDefault();
        //    var statuslogs = _db.Requeststatuslogs.Where(x => x.Requestid == requestId);
        //    ViewNotesModel model = new ViewNotesModel();
        //    if (model == null)
        //    {
        //        model.TransferNotesList = null;
        //        model.PhyscianNotes = null;
        //        model.AdminNotes = null;
        //    }

        //    if (requestNotes != null)
        //    {
        //        model.PhyscianNotes = requestNotes.Physiciannotes;
        //        model.AdminNotes = requestNotes.Adminnotes;
        //    }

        //    if (statuslogs != null)
        //    {
        //        model.TransferNotesList = statuslogs;
        //    }
        //    return model;
        //}

        public ViewNotesModel ViewNotes(int requestId)
        {

            var requestNotes = _db.Requestnotes.Where(x => x.Requestid == requestId).FirstOrDefault();
            var statuslogs = _db.Requeststatuslogs.Where(x => x.Requestid == requestId).ToList();
            ViewNotesModel model = new ViewNotesModel();
            if (model == null)
            {
                model.TransferNotesList = null;
                model.PhyscianNotes = null;
                model.AdminNotes = null;
            }

            if (requestNotes != null)
            {
                model.PhyscianNotes = requestNotes.Physiciannotes;
                model.AdminNotes = requestNotes.Adminnotes;
            }
            if (statuslogs != null)
            {
                model.TransferNotesList = statuslogs;
            }

            return model;
        }

        public bool UpdateAdminNotes(string AdminNotes, int RequestId)
        {
            var reqNotes = _db.Requestnotes.FirstOrDefault(x => x.Requestid == RequestId);
            try
            {
                if (reqNotes == null)
                {
                    Requestnote rn = new Requestnote();
                    rn.Requestid = RequestId;
                    rn.Adminnotes = AdminNotes;
                    rn.Createdby = "admin";
                    //here instead of admin , add id of the admin through which admin is loggedIn 
                    rn.Createddate = DateTime.Now;
                    _db.Requestnotes.Add(rn);
                }
                else
                {
                    reqNotes.Adminnotes = AdminNotes;
                    reqNotes.Modifieddate = DateTime.Now;
                    reqNotes.Modifiedby = "admin";
                    //here instead of admin , add id of the admin through which admin is loggedIn 
                    _db.Requestnotes.Update(reqNotes);
                }
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public CancelCaseModel CancelCase(int reqId)
        {
            var casetags = _db.Casetags.ToList();
            var request = _db.Requests.Where(x => x.Requestid == reqId).FirstOrDefault();
            CancelCaseModel model = new()
            {
                reqId = reqId,
                PatientFName = request.Firstname,
                PatientLName = request.Lastname,
                casetaglist = casetags
            };
            return model;
        }

        public bool SubmitCancelCase(CancelCaseModel cancelCaseModel)
        {
            try
            {
                var req = _db.Requests.Where(x => x.Requestid == cancelCaseModel.reqId).FirstOrDefault();
                req.Status = (int)StatusEnum.Cancelled;
                req.Casetag = cancelCaseModel.casetag.ToString();
                req.Modifieddate = DateTime.Now;
                var reqStatusLog = _db.Requeststatuslogs.Where(x => x.Requestid == cancelCaseModel.reqId).FirstOrDefault();
                if (reqStatusLog == null)
                {
                    Requeststatuslog rsl = new Requeststatuslog();
                    rsl.Requestid = (int)cancelCaseModel.reqId;
                    rsl.Status = (int)StatusEnum.Cancelled;
                    rsl.Notes = cancelCaseModel.notes;
                    rsl.Createddate = DateTime.Now;
                    _db.Requeststatuslogs.Add(rsl);
                    _db.Requests.Update(req);
                }
                else
                {
                    reqStatusLog.Status = (int)StatusEnum.Cancelled;
                    reqStatusLog.Notes = cancelCaseModel.notes;

                    _db.Requeststatuslogs.Update(reqStatusLog);
                    _db.Requests.Update(req);
                }
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ViewUploadModel GetAllDocById(int requestId)
        {

            var list = _db.Requestwisefiles.Where(x => x.Requestid == requestId).ToList();
            var reqClient = _db.Requestclients.Where(x => x.Requestid == requestId).FirstOrDefault();
            ViewUploadModel result = new()
            {
                files = list,
                firstName = reqClient.Firstname,
                lastName = reqClient.Lastname,
            };
            return result;
        }

        public Order FetchProfession()
        {
            var Healthprofessionaltype = _db.Healthprofessionaltypes.ToList();

            Order order = new()
            {
                Profession = Healthprofessionaltype

            };
            return order;
        }
        public JsonArray FetchVendors(int proffesionId)
        {
            var result = new JsonArray();
            IEnumerable<Healthprofessional> businesses = _db.Healthprofessionals.Where(prof => prof.Profession == proffesionId);

            foreach (Healthprofessional business in businesses)
            {
                result.Add(new { businessId = business.Vendorid, businessName = business.Vendorname });
            }
            return result;
        }

        public Healthprofessional VendorDetails(int selectedValue)
        {
            Healthprofessional business = _db.Healthprofessionals.First(prof => prof.Vendorid == selectedValue);

            return business;
        }

        public AssignCaseModel AssignCase(int reqId)
        {
            var regionlist = _db.Regions.ToList();
            AssignCaseModel assignCaseModel = new()
            {
                regionList = regionlist,
            };
            return assignCaseModel;
        }

        public List<Physician> GetPhysicianByRegion(int Regionid)
        {
            var physicianList = _db.Physicianregions.Where(x => x.Regionid == Regionid).Select(x => x.Physician).ToList();
            return physicianList;
        }

        public bool SubmitAssignCase(AssignCaseModel assignCaseModel)
        {
            try
            {
                var req = _db.Requests.Where(x => x.Requestid == assignCaseModel.ReqId).FirstOrDefault();
                req.Status = (int)StatusEnum.Accepted;
                req.Physicianid = assignCaseModel.selectPhysicianId;
                req.Modifieddate = DateTime.Now;

                var reqStatusLog = _db.Requeststatuslogs.Where(x => x.Requestid == assignCaseModel.ReqId).FirstOrDefault();
                if (reqStatusLog == null)
                {
                    Requeststatuslog rsl = new Requeststatuslog();
                    rsl.Requestid = (int)assignCaseModel.ReqId;
                    rsl.Status = (int)StatusEnum.Accepted;
                    rsl.Notes = assignCaseModel.description;
                    rsl.Createddate = DateTime.Now;
                    rsl.Physicianid = assignCaseModel.selectPhysicianId;
                    _db.Requeststatuslogs.Add(rsl);
                }
                else
                {
                    reqStatusLog.Status = (int)StatusEnum.Accepted;
                    reqStatusLog.Notes = assignCaseModel.description;
                    reqStatusLog.Physicianid = assignCaseModel.selectPhysicianId;

                    _db.Requeststatuslogs.Update(reqStatusLog);
                }
                _db.Requests.Update(req);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public BlockCaseModel BlockCase(int reqId)
        {
            var reqClient = _db.Requestclients.Where(x => x.Requestid == reqId).FirstOrDefault();
            BlockCaseModel model = new()
            {
                ReqId = reqId,
                firstName = reqClient.Firstname,
                lastName = reqClient.Lastname,
                reason = null
            };
            return model;
        }

        public bool SubmitBlockCase(BlockCaseModel blockCaseModel)
        {
            try
            {
                var request = _db.Requests.FirstOrDefault(r => r.Requestid == blockCaseModel.ReqId);
                if (request != null)
                {
                    if (request.Isdeleted == null)
                    {
                        request.Isdeleted = new BitArray(1);
                        request.Isdeleted[0] = true;
                        request.Status = (int)StatusEnum.Clear;
                        request.Modifieddate = DateTime.Now;

                        _db.Requests.Update(request);
                    }

                    Blockrequest blockrequest = new Blockrequest();
                    blockrequest.Phonenumber = request.Phonenumber == null ? "+91" : request.Phonenumber;
                    blockrequest.Email = request.Email;
                    blockrequest.Reason = blockCaseModel.reason;
                    blockrequest.Requestid = (int)blockCaseModel.ReqId;
                    blockrequest.Createddate = DateTime.Now;

                    _db.Blockrequests.Add(blockrequest);
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UploadFiles(List<IFormFile> files, int reqId)
        {
            try
            {
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        //if (file != null && file.Length > 0)
                        //{
                        //get file name
                        var fileName = Path.GetFileName(file.FileName);
                        //define path
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles", fileName);
                        // Copy the file to the desired location
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        Requestwisefile requestwisefile = new()
                        {
                            Filename = fileName,
                            Requestid = reqId,
                            Createddate = DateTime.Now
                        };
                        _db.Requestwisefiles.Add(requestwisefile);
                        //}
                    }
                    _db.SaveChanges();
                    return true;
                }
                else { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteFileById(int reqFileId)
        {
            try
            {
                var reqWiseFile = _db.Requestwisefiles.Where(x => x.Requestwisefileid == reqFileId).FirstOrDefault();
                if (reqWiseFile != null)
                {
                    _db.Requestwisefiles.Remove(reqWiseFile);
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteAllFiles(List<string> filenames, int reqId)
        {
            try
            {
                var list = _db.Requestwisefiles.Where(x => x.Requestid == reqId).ToList();

                foreach (var filename in filenames)
                {
                    var existFile = list.Where(x => x.Filename == filename && x.Requestid == reqId).FirstOrDefault();
                    if (existFile != null)
                    {
                        list.Remove(existFile);
                        _db.Requestwisefiles.Remove(existFile);
                    }
                }
                _db.SaveChanges();
                return true;
            }



            catch (Exception ex)
            {
                return false;
            }
        }

        //public Orderdetail SendOrderDetails(Order order)
        //{
        //    var reqData = _db.Requests.Where(x => x.Requestid == order.ReqId).FirstOrDefault();
        //    var usersData = _db.Users.Where(x => x.Userid == reqData.Userid).FirstOrDefault();
        //    var aspnetusersData = _db.Aspnetusers.Where(x => x.Id == usersData.Aspnetuserid).FirstOrDefault();
        //    Orderdetail orderDetail = new()
        //    {
        //        Vendorid = order.vendorid,
        //        Requestid = order.ReqId,
        //        Faxnumber = order.faxnumber,
        //        Email = order.email,
        //        Businesscontact = order.BusineesContact,
        //        Prescription = order.orderdetail,
        //        Noofrefill = order.refills,
        //        Createddate = DateTime.Now,
        //        Createdby = aspnetusersData.Username,
        //    };
        //    _db.Orderdetails.Add(orderDetail);
        //    _db.SaveChanges();

        //    return orderDetail;
        //}

        public bool SendOrder(Order order)
        {
            try
            {
                Orderdetail od = new Orderdetail();
                od.Vendorid = order.vendorid;
                od.Requestid = order.ReqId;
                od.Faxnumber = order.faxnumber;
                od.Email = order.email;
                od.Businesscontact = order.BusineesContact;
                od.Prescription = order.orderdetail;
                od.Noofrefill = order.refills;
                od.Createddate = DateTime.Now;
                od.Createdby = "Admin";

                _db.Orderdetails.Add(od);
                _db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ClearCase(int reqId)
        {
            try
            {
                var request = _db.Requests.FirstOrDefault(x => x.Requestid == reqId);
                if (request != null)
                {
                    request.Status = (int)StatusEnum.Clear;
                    _db.Requests.Update(request);
                    _db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public SendAgreementModel SendAgreementCase(int reqId)
        {
            var requestClient = _db.Requestclients.Where(x => x.Requestid == reqId).FirstOrDefault();
            SendAgreementModel obj = new();
            obj.Reqid = reqId;
            obj.PhoneNumber = requestClient.Phonenumber;
            obj.Email = requestClient.Email;

            return obj;
        }

        public CloseCaseModel ShowCloseCase(int reqId)
        {
            var requestClient = _db.Requestclients.FirstOrDefault(x => x.Requestid == reqId);
            var request = _db.Requests.FirstOrDefault(x => x.Requestid == reqId);
            var list = _db.Requestwisefiles.Where(x => x.Requestid == reqId).ToList();
            CloseCaseModel model = new()
            {
                reqid = reqId,
                fname = requestClient.Firstname,
                lname = requestClient.Lastname,
                email = requestClient.Email,
                phoneNo = requestClient.Phonenumber,
                files = list,
                confirmation_no = request.Confirmationnumber
            };
            return model;
        }

        public bool SaveCloseCase(CloseCaseModel model)
        {
            try
            {
                var reqClient = _db.Requestclients.FirstOrDefault(x => x.Requestid == model.reqid);
                reqClient.Phonenumber = model.phoneNo;
                reqClient.Email = model.email;
                _db.Requestclients.Update(reqClient);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool SubmitCloseCase(int ReqId)
        {
            try
            {
                var request = _db.Requests.FirstOrDefault(x => x.Requestid == ReqId);
                request.Status = (int)StatusEnum.Unpaid;
                _db.Requests.Update(request);
                _db.Requeststatuslogs.Add(new Requeststatuslog()
                {
                    Requestid = ReqId,
                    Status = (int)StatusEnum.Unpaid,
                    Notes = "Case closed and unpaid",
                    Createddate = DateTime.Now,
                });
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool AgreeAgreement(AgreementModel model)
        {
            try
            {
                var req = _db.Requests.FirstOrDefault(x => x.Requestid == model.Reqid);
                req.Status = (int)StatusEnum.MDEnRoute;
                req.Modifieddate = DateTime.Now;

                Requeststatuslog rsl = new Requeststatuslog();
                rsl.Requestid = req.Requestid;
                rsl.Status = (int)StatusEnum.MDEnRoute;
                rsl.Createddate = DateTime.Now;
                rsl.Notes = "Agreement accepted by patient";

                _db.Requeststatuslogs.Add(rsl);
                _db.Requests.Update(req);
                _db.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public int GetStatusForReviewAgreement(int reqId)
        {
            var status = (int)_db.Requests.Where(x => x.Requestid == reqId).Select(x => x.Status).FirstOrDefault();
            return status;
        }


        public AgreementModel CancelAgreement(int reqId)
        {
            var requestclient = _db.Requestclients.FirstOrDefault(x => x.Requestid == reqId);
            AgreementModel model = new()
            {
                Reqid = reqId,
                fname = requestclient.Firstname,
                lname = requestclient.Lastname,
            };
            return model;
        }

        public bool SubmitCancelAgreement(AgreementModel model)
        {
            try
            {
                var request = _db.Requests.FirstOrDefault(x => x.Requestid == model.Reqid);
                if (request != null)
                {
                    request.Status = (int)StatusEnum.CancelledByPatient;
                    request.Modifieddate = DateTime.Now;
                    Requeststatuslog rsl = new Requeststatuslog();
                    rsl.Requestid = request.Requestid;
                    rsl.Status = (int)StatusEnum.CancelledByPatient;
                    rsl.Notes = model.Reason;
                    rsl.Createddate = DateTime.Now;

                    _db.Requests.Update(request);
                    _db.Requeststatuslogs.Add(rsl);
                    _db.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public EncounterFormModel EncounterForm(int reqId)
        {
            var reqClient = _db.Requestclients.FirstOrDefault(x => x.Requestid == reqId);
            var encForm = _db.Encounterforms.FirstOrDefault(x => x.Requestid == reqId);
            EncounterFormModel ef = new EncounterFormModel();
            ef.reqid = reqId;
            ef.FirstName = reqClient.Firstname;
            ef.LastName = reqClient.Lastname;
            ef.Location = reqClient.Street + reqClient.City + reqClient.State + reqClient.Zipcode;
            //ef.BirthDate = new DateTime((int)(reqClient.Intyear), Convert.ToInt16(reqClient.Strmonth), (int)(reqClient.Intdate)).ToString("yyyy-MM-dd");
            ef.PhoneNumber = reqClient.Phonenumber;
            ef.Email = reqClient.Email;
            if (encForm != null)
            {
                ef.HistoryIllness = encForm.Illnesshistory;
                ef.MedicalHistory = encForm.Medicalhistory;
                //ef.Date = encForm.Intdate;
                ef.Medications = encForm.Medications;
                ef.Allergies = encForm.Allergies;
                ef.Temp = encForm.Temperature;
                ef.Hr = encForm.Heartrate;
                ef.Rr = encForm.Respirationrate;
                ef.BpS = encForm.Bloodpressuresystolic;
                ef.BpD = encForm.Bloodpressurediastolic;
                ef.O2 = encForm.Oxygenlevel;
                ef.Pain = encForm.Pain;
                ef.Heent = encForm.Heent;
                ef.Cv = encForm.Cardiovascular;
                ef.Chest = encForm.Chest;
                ef.Abd = encForm.Abdomen;
                ef.Extr = encForm.Extremities;
                ef.Skin = encForm.Skin;
                ef.Neuro = encForm.Neuro;
                ef.Other = encForm.Other;
                ef.Diagnosis = encForm.Diagnosis;
                ef.TreatmentPlan = encForm.Treatmentplan;
                ef.MedicationDispensed = encForm.Medicationsdispensed;
                ef.Procedures = encForm.Procedures;
                ef.FollowUp = encForm.Followup;
            }
            return ef;
        }

        public bool SubmitEncounterForm(EncounterFormModel model)
        {
            try
            {
                //concludeEncounter _obj = new concludeEncounter();

                var ef = _db.Encounterforms.FirstOrDefault(r => r.Requestid == model.reqid);

                if (ef == null)
                {
                    Encounterform _encounter = new Encounterform()
                    {
                        Requestid = model.reqid,
                        Firstname = model.FirstName,
                        Lastname = model.LastName,
                        Location = model.Location,
                        Phonenumber = model.PhoneNumber,
                        Email = model.Email,
                        Illnesshistory = model.HistoryIllness,
                        Medicalhistory = model.MedicalHistory,
                        //date = model.Date,
                        Medications = model.Medications,
                        Allergies = model.Allergies,
                        Temperature = model.Temp,
                        Heartrate = model.Hr,
                        Respirationrate = model.Rr,
                        Bloodpressuresystolic = model.BpS,
                        Bloodpressurediastolic = model.BpD,
                        Oxygenlevel = model.O2,
                        Pain = model.Pain,
                        Heent = model.Heent,
                        Cardiovascular = model.Cv,
                        Chest = model.Chest,
                        Abdomen = model.Abd,
                        Extremities = model.Extr,
                        Skin = model.Skin,
                        Neuro = model.Neuro,
                        Other = model.Other,
                        Diagnosis = model.Diagnosis,
                        Treatmentplan = model.TreatmentPlan,
                        Medicationsdispensed = model.MedicationDispensed,
                        Procedures = model.Procedures,
                        Followup = model.FollowUp,
                        Isfinalized = false
                    };

                    _db.Encounterforms.Add(_encounter);

                    //_obj.indicate = true;
                }
                else
                {
                    var efdetail = _db.Encounterforms.FirstOrDefault(x => x.Requestid == model.reqid);

                    efdetail.Requestid = model.reqid;
                    efdetail.Illnesshistory = model.HistoryIllness;
                    efdetail.Medicalhistory = model.MedicalHistory;
                    //efdetail.Date = model.Date;
                    efdetail.Medications = model.Medications;
                    efdetail.Allergies = model.Allergies;
                    efdetail.Temperature = model.Temp;
                    efdetail.Heartrate = model.Hr;
                    efdetail.Respirationrate = model.Rr;
                    efdetail.Bloodpressuresystolic = model.BpS;
                    efdetail.Bloodpressurediastolic = model.BpD;
                    efdetail.Oxygenlevel = model.O2;
                    efdetail.Pain = model.Pain;
                    efdetail.Heent = model.Heent;
                    efdetail.Cardiovascular = model.Cv;
                    efdetail.Chest = model.Chest;
                    efdetail.Abdomen = model.Abd;
                    efdetail.Extremities = model.Extr;
                    efdetail.Skin = model.Skin;
                    efdetail.Neuro = model.Neuro;
                    efdetail.Other = model.Other;
                    efdetail.Diagnosis = model.Diagnosis;
                    efdetail.Treatmentplan = model.TreatmentPlan;
                    efdetail.Medicationsdispensed = model.MedicationDispensed;
                    efdetail.Procedures = model.Procedures;
                    efdetail.Followup = model.FollowUp;
                    efdetail.Modifieddate = DateTime.Now;
                    ef.Isfinalized = false;
                    _db.Encounterforms.Update(efdetail);
                    // _obj.indicate = true;
                };


                _db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public MyProfileModel MyProfile(string sessionEmail)
        {
            var myProfileMain = _db.Admins.Where(x => x.Email == sessionEmail).Select(x => new MyProfileModel()
            {
                fname = x.Firstname,
                lname = x.Lastname,
                email = x.Email,
                confirm_email = x.Email,
                mobile_no = x.Mobile,
                addr1 = x.Address1,
                addr2 = x.Address2,
                city = x.City,
                zip = x.Zip,
                state = _db.Regions.Where(r => r.Regionid == x.Regionid).Select(r => r.Name).First(),
                roles = _db.Aspnetroles.ToList(),
            }).ToList().FirstOrDefault();

            var aspnetuser = _db.Aspnetusers.Where(r => r.Email == sessionEmail).First();
            //myProfileMain.username = aspnetuser.Username;
            //myProfileMain.password = aspnetuser.Passwordhash;

            return myProfileMain;
        }

        public bool VerifyState(string state)
        {

            var stateMain = _db.Regions.Where(r => r.Name.ToLower() == state.ToLower().Trim()).FirstOrDefault();

            if (stateMain == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CreateRequest(CreateRequestModel model, string sessionEmail)
        {
            CreateRequestModel _create = new CreateRequestModel();

            var stateMain = _db.Regions.Where(r => r.Name.ToLower() == model.state.ToLower().Trim()).FirstOrDefault();

            if (stateMain == null)
            {
                return false;
            }
            else
            {
                Request _req = new Request();
                Requestclient _reqClient = new Requestclient();
                User _user = new User();
                Aspnetuser _asp = new Aspnetuser();
                Requestnote _note = new Requestnote();

                var admin = _db.Admins.Where(r => r.Email == sessionEmail).Select(r => r).First();

                var existUser = _db.Aspnetusers.FirstOrDefault(r => r.Email == model.email);

                if (existUser == null)
                {
                    _asp.Id = Guid.NewGuid().ToString();
                    _asp.Username = model.firstname + "_" + model.lastname;
                    _asp.Email = model.email;
                    _asp.Phonenumber = model.phone;
                    _asp.Createddate = DateTime.Now;
                    _db.Aspnetusers.Add(_asp);
                    _db.SaveChanges();

                    _user.Aspnetuserid = _asp.Id;
                    _user.Firstname = model.firstname;
                    _user.Lastname = model.lastname;
                    _user.Email = model.email;
                    _user.Mobile = model.phone;
                    _user.City = model.city;
                    _user.State = model.state;
                    _user.Street = model.street;
                    _user.Zip = model.zipcode;
                    _user.Strmonth = model.dateofbirth.Substring(5, 2);
                    _user.Intdate = Convert.ToInt16(model.dateofbirth.Substring(8, 2));
                    _user.Intyear = Convert.ToInt16(model.dateofbirth.Substring(0, 4));
                    _user.Createdby = _asp.Id;
                    _user.Createddate = DateTime.Now;
                    _user.Regionid = stateMain.Regionid;
                    _db.Users.Add(_user);
                    _db.SaveChanges();

                    string registrationLink = "http://localhost:5145/Home/CreateAccount?aspuserId=" + _asp.Id;

                    try
                    {
                        //SendRegistrationEmailCreateRequest(data.email, registrationLink);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                _req.Requesttypeid = (int)RequestTypeEnum.Patient;
                _req.Userid = _db.Users.Where(x => x.Email == sessionEmail).Select(x => x.Userid).First();
                _req.Firstname = admin.Firstname;
                _req.Lastname = admin.Lastname;
                _req.Phonenumber = admin.Mobile;
                _req.Email = admin.Email;
                _req.Status = (int)StatusEnum.Unassigned;
                _req.Confirmationnumber = admin.Firstname.Substring(0, 1) + DateTime.Now.ToString().Substring(0, 19);
                _req.Createddate = DateTime.Now;
                _req.Isurgentemailsent = new BitArray(1);
                _req.Isurgentemailsent[0] = false;

                _db.Requests.Add(_req);
                _db.SaveChanges();

                _reqClient.Requestid = _req.Requestid;
                _reqClient.Firstname = model.firstname;
                _reqClient.Lastname = model.lastname;
                _reqClient.Phonenumber = model.phone;
                _reqClient.Strmonth = model.dateofbirth.Substring(5, 2);
                _reqClient.Intdate = Convert.ToInt16(model.dateofbirth.Substring(8, 2));
                _reqClient.Intyear = Convert.ToInt16(model.dateofbirth.Substring(0, 4));
                _reqClient.Street = model.street;
                _reqClient.City = model.city;
                _reqClient.State = model.state;
                _reqClient.Zipcode = model.zipcode;
                _reqClient.Regionid = stateMain.Regionid;
                _reqClient.Email = model.email;

                _db.Requestclients.Add(_reqClient);
                _db.SaveChanges();

                _note.Requestid = _req.Requestid;
                _note.Adminnotes = model.admin_notes;
                _note.Createdby = _db.Aspnetusers.Where(r => r.Email == sessionEmail).Select(r => r.Id).First();
                _note.Createddate = DateTime.Now;
                _db.Requestnotes.Add(_note);
                _db.SaveChanges();

                return true;
            }


        }

    }
}