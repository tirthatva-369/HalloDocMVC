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
            var aspNetUser = _db.Aspnetusers.FirstOrDefault(x => x.Email == email);
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

        public ViewCaseViewModel ViewCase(int reqClientId)
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
                RequestTypeId = requestData.Requesttypeid,
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
    }
}