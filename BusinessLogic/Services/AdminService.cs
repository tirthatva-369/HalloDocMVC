using BusinessLogic.Interfaces;
using DataAccess.Models;
using DataAccess.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DataModels;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using DataAccess.Enums;
using DataAccess.Enum;
using System.Collections;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;

namespace BusinessLogic.Services
{
    public class AdminService : IAdminInterface
    {
        private readonly ApplicationDbContext _db;

        public AdminService(ApplicationDbContext db)
        {
            _db = db;
        }

        public Aspnetuser GetAspnetuser(string email)
        {
            var aspNetUser = _db.Aspnetusers.Include(x => x.Aspnetuserroles).FirstOrDefault(x => x.Email == email);
            return aspNetUser;
        }

        public List<AdminDashTableModel> GetRequestsByStatus(int tabNo)
        {
            var query = from r in _db.Requests
                        join rc in _db.Requestclients on r.Requestid equals rc.Requestid
                        select new AdminDashTableModel
                        {
                            reqId = r.Requestid,
                            firstName = rc.Firstname,
                            lastName = rc.Lastname,
                            intDate = rc.Intdate,
                            intYear = rc.Intyear,
                            strMonth = rc.Strmonth,
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

            var result = query.ToList();

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
            return result;
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

        public ViewCaseViewModel ViewCase(int reqClientId, int RequestTypeId)
        {
            var data = _db.Requestclients.FirstOrDefault(x => x.Requestclientid == reqClientId);
            var requestData = _db.Requests.FirstOrDefault(x => x.Requestid == data.Requestid);
            var userData = _db.Users.FirstOrDefault(x => x.Userid == requestData.Userid);
            var regionData = _db.Regions.FirstOrDefault(x => x.Regionid == data.Regionid);
            var reqtypeData = _db.Requesttypes.FirstOrDefault(x => x.Requesttypeid == requestData.Requesttypeid);

            var viewdata = new ViewCaseViewModel
            {
                RequestId = requestData.Requestid,
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
                DateOfBirth = new DateTime(Convert.ToInt32(userData.Intyear), DateTime.ParseExact(userData.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(userData.Intdate)),
                IntYear = userData.Intyear,
                IntDate = userData.Intdate,
                StrMonth = userData.Strmonth,

            };
            return viewdata;
        }

        public ViewNotesModel ViewNotes(int requestId)
        {
            var reqnotesData = _db.Requestnotes.FirstOrDefault(x => x.Requestid == requestId);
            var reqstatusData = _db.Requeststatuslogs.FirstOrDefault(x => x.Requestid == requestId);
            var viewnotes = new ViewNotesModel
            {
                TransferNotes = reqstatusData != null ? reqstatusData.Notes : null,
                PhyscianNotes = reqnotesData != null ? reqnotesData.Physiciannotes : null,
                AdminNotes = reqnotesData != null ? reqnotesData.Adminnotes : null,
                RequestId = requestId
            };
            return viewnotes;
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
                    _db.SaveChanges();
                }
                else
                {
                    reqNotes.Adminnotes = AdminNotes;
                    reqNotes.Modifieddate = DateTime.Now;
                    reqNotes.Modifiedby = "admin";
                    //here instead of admin , add id of the admin through which admin is loggedIn 
                    _db.Requestnotes.Update(reqNotes);
                    _db.SaveChanges();
                }
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
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    reqStatusLog.Status = (int)StatusEnum.Cancelled;
                    reqStatusLog.Notes = cancelCaseModel.notes;

                    _db.Requeststatuslogs.Update(reqStatusLog);
                    _db.Requests.Update(req);
                    _db.SaveChanges();
                    return true;
                }
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
                    _db.Requeststatuslogs.Add(rsl);
                    _db.Requests.Update(req);
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    reqStatusLog.Status = (int)StatusEnum.Accepted;
                    reqStatusLog.Notes = assignCaseModel.description;
                    _db.Requeststatuslogs.Update(reqStatusLog);
                    _db.Requests.Update(req);
                    _db.SaveChanges();
                    return true;
                }
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
                        if (file != null && file.Length > 0)
                        {
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
                        }
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

        public Orderdetail SendOrderDetails(Order order)
        {
            var reqData = _db.Requests.Where(x => x.Requestid == order.ReqId).FirstOrDefault();
            var usersData = _db.Users.Where(x => x.Userid == reqData.Userid).FirstOrDefault();
            var aspnetusersData = _db.Aspnetusers.Where(x => x.Id == usersData.Aspnetuserid).FirstOrDefault();
            Orderdetail orderDetail = new()
            {
                Vendorid = order.vendorid,
                Requestid = order.ReqId,
                Faxnumber = order.faxnumber,
                Email = order.email,
                Businesscontact = order.BusineesContact,
                Prescription = order.orderdetail,
                Noofrefill = order.refills,
                Createddate = DateTime.Now,
                Createdby = aspnetusersData.Username,
            };
            _db.Orderdetails.Add(orderDetail);
            _db.SaveChanges();

            return orderDetail;
        }

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
    }
}