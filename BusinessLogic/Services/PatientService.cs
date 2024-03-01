using BusinessLogic.Interfaces;
using DataAccess.Models;
using System.Collections;
using DataAccess.DataContext;
using DataAccess.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;

namespace BusinessLogic.Services
{
    public class PatientService : IPatientInterface
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _env;


        public PatientService(ApplicationDbContext db, IHostingEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public void AddPatientInfo(PatientRequestModel patientRequestModel)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var aspnetuser = _db.Aspnetusers.Where(m => m.Email == patientRequestModel.email).FirstOrDefault();
                    User user = new User();
                    if (aspnetuser == null)
                    {
                        Aspnetuser aspnetuser1 = new Aspnetuser();
                        aspnetuser1.Id = Guid.NewGuid().ToString();
                        aspnetuser1.Passwordhash = patientRequestModel.password;
                        aspnetuser1.Email = patientRequestModel.email;
                        string username = patientRequestModel.firstName + patientRequestModel.lastName;
                        aspnetuser1.Username = username;
                        aspnetuser1.Phonenumber = patientRequestModel.phoneNo;
                        aspnetuser1.Modifieddate = DateTime.Now;
                        _db.Aspnetusers.Add(aspnetuser1);

                        user.Aspnetuserid = aspnetuser1.Id;
                        user.Firstname = patientRequestModel.firstName;
                        user.Lastname = patientRequestModel.lastName;
                        user.Email = patientRequestModel.email;
                        user.Mobile = patientRequestModel.phoneNo;
                        user.Street = patientRequestModel.street;
                        user.City = patientRequestModel.city;
                        user.State = patientRequestModel.state;
                        user.Zip = patientRequestModel.zipCode;
                        user.Createdby = patientRequestModel.firstName + patientRequestModel.lastName;
                        user.Intyear = patientRequestModel.dateOfBirth.Year;
                        user.Intdate = patientRequestModel.dateOfBirth.Day;
                        user.Strmonth = patientRequestModel.dateOfBirth.ToString("MMM");
                        user.Modifieddate = DateTime.Now;
                        user.Status = 1;
                        user.Regionid = 1;

                        _db.Users.Add(user);
                    }
                    else
                    {
                        user = _db.Users.Where(m => m.Email == patientRequestModel.email).FirstOrDefault();
                    }

                    Request request = new Request();
                    request.Requesttypeid = 1;
                    request.Status = 1;
                    request.Createddate = DateTime.Now;
                    request.Modifieddate = DateTime.Now;
                    request.Isurgentemailsent = new BitArray(1);
                    request.Firstname = patientRequestModel.firstName;
                    request.Lastname = patientRequestModel.lastName;
                    request.Phonenumber = patientRequestModel.phoneNo;
                    request.Email = patientRequestModel.email;
                    request.User = user;

                    _db.Requests.Add(request);
                    _db.SaveChanges();

                    Requestclient info = new Requestclient();
                    info.Request = request;
                    info.Notes = patientRequestModel.symptoms;
                    info.Firstname = patientRequestModel.firstName;
                    info.Lastname = patientRequestModel.lastName;
                    info.Phonenumber = patientRequestModel.phoneNo;
                    info.Email = patientRequestModel.email;
                    info.Street = patientRequestModel.street;
                    info.City = patientRequestModel.city;
                    info.State = patientRequestModel.state;
                    info.Zipcode = patientRequestModel.zipCode;
                    info.Regionid = 1;
                    info.Intyear = patientRequestModel.dateOfBirth.Year;
                    info.Intdate = patientRequestModel.dateOfBirth.Day;
                    info.Strmonth = patientRequestModel.dateOfBirth.Month.ToString("MMM");
                    info.Address = patientRequestModel.street + ", " + patientRequestModel.city + ", " + patientRequestModel.state + ", " + patientRequestModel.zipCode;

                    _db.Requestclients.Add(info);
                    _db.SaveChanges();
                    if (patientRequestModel.file != null)
                    {
                        uploadFile(patientRequestModel.file, request.Requestid);
                    }
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        public void uploadFile(List<IFormFile> upload, int id)
        {
            foreach (var item in upload)
            {
                string path = _env.WebRootPath + "/UploadedFiles/" + item.FileName;
                FileStream stream = new FileStream(path, FileMode.Create);

                item.CopyTo(stream)
;
                Requestwisefile requestwisefile = new Requestwisefile
                {
                    Requestid = id,
                    Filename = item.FileName,
                    Createddate = DateTime.Now,
                };
                _db.Add(requestwisefile);
                _db.SaveChanges();
            }
        }


        public void AddFamilyRequest(FamilyRequestModel familyRequestModel)
        {
            Request request = new Request();
            request.Requesttypeid = 2;
            request.Status = 1;
            request.Createddate = DateTime.Now;
            request.Isurgentemailsent = new BitArray(1);
            request.Firstname = familyRequestModel.firstName;
            request.Lastname = familyRequestModel.lastName;
            request.Phonenumber = familyRequestModel.phoneNo;
            request.Email = familyRequestModel.email;
            request.Relationname = familyRequestModel.relation;

            _db.Requests.Add(request);
            _db.SaveChanges();

            Requestclient info = new Requestclient();
            info.Requestid = request.Requestid;
            info.Notes = familyRequestModel.symptoms;
            info.Firstname = familyRequestModel.patientFirstName;
            info.Lastname = familyRequestModel.patientLastName;
            info.Phonenumber = familyRequestModel.patientPhoneNo;
            info.Email = familyRequestModel.patientEmail;
            info.Street = familyRequestModel.street;
            info.City = familyRequestModel.city;
            info.State = familyRequestModel.state;
            info.Zipcode = familyRequestModel.zipCode;
            info.Regionid = 1;

            _db.Requestclients.Add(info);
            _db.SaveChanges();
        }

        public void AddConciergeRequest(ConciergeRequestModel conciergeReqModel)
        {
            Request request = new Request();
            request.Requesttypeid = 3;
            request.Status = 1;
            request.Createddate = DateTime.Now;
            request.Isurgentemailsent = new BitArray(1);
            request.Firstname = conciergeReqModel.firstName;
            request.Lastname = conciergeReqModel.lastName;
            request.Phonenumber = conciergeReqModel.phoneNo;
            request.Email = conciergeReqModel.email;
            request.Relationname = "Concierge";

            _db.Requests.Add(request);
            _db.SaveChanges();

            Requestclient info = new Requestclient();
            info.Requestid = request.Requestid;
            info.Notes = conciergeReqModel.symptoms;
            info.Firstname = conciergeReqModel.patientFirstName;
            info.Lastname = conciergeReqModel.patientLastName;
            info.Phonenumber = conciergeReqModel.patientPhoneNo;
            info.Email = conciergeReqModel.patientEmail;
            info.Regionid = 1;

            _db.Requestclients.Add(info);
            _db.SaveChanges();

            Concierge concierge = new Concierge();
            concierge.Conciergename = conciergeReqModel.firstName + " " + conciergeReqModel.lastName;
            concierge.Createddate = DateTime.Now;
            concierge.Regionid = 1;
            concierge.Street = conciergeReqModel.street;
            concierge.City = conciergeReqModel.city;
            concierge.State = conciergeReqModel.state;
            concierge.Zipcode = conciergeReqModel.zipCode;

            _db.Concierges.Add(concierge);
            _db.SaveChanges();

            Requestconcierge reqCon = new Requestconcierge();
            reqCon.Requestid = request.Requestid;
            reqCon.Conciergeid = concierge.Conciergeid;

            _db.Requestconcierges.Add(reqCon);
            _db.SaveChanges();

        }

        public void AddBusinessRequest(BusinessRequestModel businessReqModel)
        {
            Request request = new Request();
            request.Requesttypeid = 4;
            request.Status = 1;
            request.Createddate = DateTime.Now;
            request.Isurgentemailsent = new BitArray(1);
            request.Firstname = businessReqModel.firstName;
            request.Lastname = businessReqModel.lastName;
            request.Phonenumber = businessReqModel.phoneNo;
            request.Email = businessReqModel.email;
            request.Relationname = "Business";

            _db.Requests.Add(request);
            _db.SaveChanges();

            Requestclient info = new Requestclient();
            info.Requestid = request.Requestid;
            info.Notes = businessReqModel.symptoms;
            info.Firstname = businessReqModel.patientFirstName;
            info.Lastname = businessReqModel.patientLastName;
            info.Phonenumber = businessReqModel.patientPhoneNo;
            info.Email = businessReqModel.patientEmail;
            info.Regionid = 1;

            _db.Requestclients.Add(info);
            _db.SaveChanges();

            Business business = new Business();
            business.Createddate = DateTime.Now;
            business.Name = businessReqModel.businessName;
            business.Phonenumber = businessReqModel.phoneNo;
            business.City = businessReqModel.city;
            business.Zipcode = businessReqModel.zipCode;
            business.Createdby = businessReqModel.businessName;

            _db.Businesses.Add(business);
            _db.SaveChanges();

            Requestbusiness requestbusiness = new Requestbusiness();
            requestbusiness.Businessid = business.Businessid;
            requestbusiness.Requestid = request.Requestid;

            _db.Requestbusinesses.Add(requestbusiness);
            _db.SaveChanges();
        }

        public Task<bool> IsEmailExists(string email)
        {
            bool isExist = _db.Users.Any(x => x.Email == email);
            if (isExist)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public List<PatientDashboard> GetPatientInfos()
        {
            var user = _db.Requests.Where(x => x.Email == "1@g.c").FirstOrDefault();
            return new List<PatientDashboard>
            {
                new PatientDashboard {createdDate = user.Createddate, currentStatus = "Test", document = "test"},
                new PatientDashboard {createdDate = DateTime.Now, currentStatus = "pending", document="myname.jpg"},
                new PatientDashboard {createdDate = DateTime.Now, currentStatus = "active", document="hername.jpg"}
            };
        }

        public List<MedicalHistory> GetMedicalHistory(User user)
        {
            var medicalhistory = (from request in _db.Requests
                                  join requestfile in _db.Requestwisefiles
                                  on request.Requestid equals requestfile.Requestid
                                  where request.Email == user.Email && request.Email != null
                                  group requestfile by request.Requestid into groupedFiles
                                  select new MedicalHistory
                                  {
                                      FirstName = user.Firstname,
                                      LastName = user.Lastname,
                                      PhoneNumber = user.Mobile,
                                      Email = user.Email,
                                      Street = user.Street,
                                      City = user.City,
                                      State = user.State,
                                      ZipCode = user.Zip,
                                      reqId = groupedFiles.Select(x => x.Request.Requestid).FirstOrDefault(),
                                      createdDate = groupedFiles.Select(x => x.Request.Createddate).FirstOrDefault(),
                                      currentStatus = groupedFiles.Select(x => x.Request.Status).FirstOrDefault().ToString(),
                                      document = groupedFiles.Select(x => x.Filename.ToString()).ToList(),
                                      DateOfBirth = new DateTime(Convert.ToInt32(user.Intyear), DateTime.ParseExact(user.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(user.Intdate)),
                                      IntYear = user.Intyear,
                                      IntDate = user.Intdate,
                                      StrMonth = user.Strmonth,
                                  }).ToList();
            return medicalhistory;
        }

        public IQueryable<Requestwisefile>? GetAllDocById(int requestId)
        {
            var data = from request in _db.Requestwisefiles
                       where request.Requestid == requestId
                       select request;
            return data;
        }
    }
}