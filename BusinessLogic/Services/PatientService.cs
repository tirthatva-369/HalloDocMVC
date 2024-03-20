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
using Humanizer;
using DataAccess.Enum;
using DataAccess.Enums;
using Microsoft.Build.Framework;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BusinessLogic.Services
{
    public class PatientService : IPatientInterface
    {
        private readonly ApplicationDbContext _db;


        public PatientService(ApplicationDbContext db)
        {
            _db = db;
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
                        aspnetuser1.Ip = GetLocalIPv4(NetworkInterfaceType.Ethernet);
                        _db.Aspnetusers.Add(aspnetuser1);
                        _db.SaveChanges();


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
                        user.Createddate = DateTime.Now;
                        user.Modifieddate = DateTime.Now;
                        user.Status = (int)StatusEnum.Unassigned;
                        user.Regionid = 1;
                        user.Ip = GetLocalIPv4(NetworkInterfaceType.Ethernet);

                        _db.Users.Add(user);
                        _db.SaveChanges();
                    }
                    else
                    {
                        user = _db.Users.Where(m => m.Email == patientRequestModel.email).FirstOrDefault();
                    }

                    var regionData = _db.Regions.Where(x => x.Regionid == user.Regionid).FirstOrDefault();
                    Request request = new Request();
                    request.Requesttypeid = (int)RequestTypeEnum.Patient; ;
                    request.Status = (int)StatusEnum.Unassigned;
                    request.Createddate = DateTime.Now;
                    request.Modifieddate = DateTime.Now;
                    request.Isurgentemailsent = new BitArray(1);
                    request.Firstname = patientRequestModel.firstName;
                    request.Lastname = patientRequestModel.lastName;
                    request.Phonenumber = patientRequestModel.phoneNo;
                    request.Email = patientRequestModel.email;
                    request.User = user;
                    request.Ip = GetLocalIPv4(NetworkInterfaceType.Ethernet);

                    _db.Requests.Add(request);
                    _db.SaveChanges();

                    var count = (from req in _db.Requests where req.Userid == user.Userid && req.Createddate.Date == DateTime.Now.Date select req).Count();

                    request.Confirmationnumber = $"{regionData.Abbreviation.Substring(0, 2)}{request.Createddate.Day.ToString().PadLeft(2, '0')}{request.Createddate.Month.ToString().PadLeft(2, '0')}{(request.Createddate.Year % 100).ToString().PadLeft(2, '0')}{user.Lastname.ToUpper().Substring(0, 2)}{user.Firstname.ToUpper().Substring(0, 2)}{count.ToString().PadLeft(4, '0')}";

                    _db.Requests.Update(request);
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
                    info.Strmonth = patientRequestModel.dateOfBirth.ToString("MMM");
                    info.Address = patientRequestModel.street + ", " + patientRequestModel.city + ", " + patientRequestModel.state + ", " + patientRequestModel.zipCode;
                    info.Ip = GetLocalIPv4(NetworkInterfaceType.Ethernet);

                    _db.Requestclients.Add(info);
                    _db.SaveChanges();

                    if (patientRequestModel.file != null)
                    {
                        foreach (IFormFile file in patientRequestModel.file)
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
                                    Requestid = request.Requestid,
                                    Createddate = DateTime.Now
                                };

                                _db.Requestwisefiles.Add(requestwisefile);
                                _db.SaveChanges();
                            };
                        }
                    }
                    else
                    {
                        _db.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
        }

        public void AddFamilyRequest(FamilyRequestModel familyRequestModel)
        {
            Request request = new Request();
            request.Requesttypeid = (int)RequestTypeEnum.Family;
            request.Status = (int)StatusEnum.Unassigned;
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
            business.Createdby = "1";

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

        public MedicalHistoryList GetMedicalHistory(int userid)
        {
            var user = _db.Users.FirstOrDefault(x => x.Userid == userid);
            var medicalhistory = (from request in _db.Requests
                                  join requestfile in _db.Requestwisefiles
                                  on request.Requestid equals requestfile.Requestid
                                  where request.Email == user.Email && request.Email != null
                                  group requestfile by request.Requestid into groupedFiles
                                  select new MedicalHistory
                                  {
                                      reqId = groupedFiles.Select(x => x.Request.Requestid).FirstOrDefault(),
                                      createdDate = groupedFiles.Select(x => x.Request.Createddate).FirstOrDefault(),
                                      currentStatus = groupedFiles.Select(x => x.Request.Status).FirstOrDefault(),
                                      document = groupedFiles.Select(x => x.Filename.ToString()).ToList(),
                                      ConfirmationNumber = groupedFiles.Select(x => x.Request.Confirmationnumber).FirstOrDefault(),
                                  }).ToList();
            MedicalHistoryList medicalHistoryList = new()
            {
                medicalHistoriesList = medicalhistory,
                id = userid,
                firstName = user.Firstname,
                lastName = user.Lastname
            };
            return medicalHistoryList;
        }

        public DocumentModel GetAllDocById(int requestId)
        {
            var list = _db.Requestwisefiles.Where(x => x.Requestid == requestId).ToList();
            var reqClient = _db.Requestclients.Where(x => x.Requestid == requestId).FirstOrDefault();
            var req = _db.Requests.Where(x => x.Requestid == requestId).FirstOrDefault();
            DocumentModel result = new()
            {
                files = list,
                firstName = reqClient.Firstname,
                lastName = reqClient.Lastname,
                ConfirmationNumber = req.Confirmationnumber,
            };
            return result;
        }

        public Profile GetProfile(int userid)
        {
            var user = _db.Users.FirstOrDefault(x => x.Userid == userid);
            Profile profile = new Profile();
            profile.FirstName = user.Firstname;
            profile.LastName = user.Lastname;
            profile.Email = user.Email;
            profile.PhoneNo = user.Mobile;
            profile.State = user.State;
            profile.City = user.City;
            profile.Street = user.Street;
            profile.ZipCode = user.Zip;
            profile.DateOfBirth = new DateTime(Convert.ToInt32(user.Intyear), DateTime.ParseExact(user.Strmonth, "MMM", CultureInfo.InvariantCulture).Month, Convert.ToInt32(user.Intdate));
            profile.isMobileCheck = user.Ismobile[0] ? 1 : 0;

            return profile;
        }

        public bool EditProfile(Profile profile)
        {
            try
            {
                var existingUser = _db.Users.Where(x => x.Userid == profile.userId).FirstOrDefault();
                if (existingUser != null)
                {
                    existingUser.Firstname = profile.FirstName;
                    existingUser.Lastname = profile.LastName;
                    existingUser.Mobile = profile.PhoneNo;
                    existingUser.Street = profile.Street;
                    existingUser.City = profile.City;
                    existingUser.State = profile.State;
                    existingUser.Zip = profile.ZipCode;
                    existingUser.Intdate = profile.DateOfBirth.Day;
                    existingUser.Strmonth = profile.DateOfBirth.ToString("MMM");
                    existingUser.Intyear = profile.DateOfBirth.Year;

                    existingUser.Ismobile[0] = profile.isMobileCheck == 1 ? true : false;

                    _db.Users.Update(existingUser);
                    _db.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex) { return false; }
        }

        public bool UploadDocuments(List<IFormFile> files, int reqId)
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

        public string GetLocalIPv4(NetworkInterfaceType _type)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }
    }
}
