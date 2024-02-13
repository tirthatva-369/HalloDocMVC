using BusinessLogic.Interfaces;
using DataAccess.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DataContext;
using DataAccess.DataModels;

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
            Request request = new Request();
            request.Requesttypeid = 2;
            request.Status = 1;
            request.Createddate = DateTime.Now;
            request.Isurgentemailsent = new BitArray(1);
            request.Firstname = patientRequestModel.firstName;
            request.Lastname = patientRequestModel.lastName;
            request.Phonenumber = patientRequestModel.phoneNo;
            request.Email = patientRequestModel.email;

            _db.Requests.Add(request);
            _db.SaveChanges();

            Requestclient info = new Requestclient();
            info.Requestid = request.Requestid;
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

            var user = _db.Aspnetusers.Where(x => x.Id == "1").FirstOrDefault();

            User u = new User();
            u.Aspnetuserid = user.Id;
            u.Firstname = patientRequestModel.firstName;
            u.Lastname = patientRequestModel.lastName;
            u.Email = patientRequestModel.email;
            u.Mobile = patientRequestModel.phoneNo;
            u.Street = patientRequestModel.street;
            u.City = patientRequestModel.city;
            u.State = patientRequestModel.state;
            u.Zip = patientRequestModel.zipCode;
            u.Createdby = user.Username;
            u.Createddate = DateTime.Now;

            _db.Users.Add(u);
            _db.SaveChanges();

            _db.Requestclients.Add(info);
            _db.SaveChanges();
        }

        public void AddFamilyRequest(FamilyRequestModel familyRequestModel)
        {
            Request request = new Request();
            request.Requesttypeid = 3;
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

            _db.Requestclients.Add(info);
            _db.SaveChanges();
        }


        public void AddConciergeRequest(ConciergeRequestModel conciergeReqModel)
        {
            Request request = new Request();
            request.Requesttypeid = 4;
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
            request.Requesttypeid = 1;
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

            _db.Requestclients.Add(info);
            _db.SaveChanges();

            Business business = new Business();
            business.Createddate = DateTime.Now;
            business.Name = businessReqModel.businessName;
            business.Phonenumber = businessReqModel.phoneNo;
            business.City = businessReqModel.city;
            business.Zipcode = businessReqModel.zipCode;

            _db.Businesses.Add(business);
            _db.SaveChanges();

            Requestbusiness requestbusiness = new Requestbusiness();
            requestbusiness.Businessid = business.Businessid;
            requestbusiness.Requestid = request.Requestid;

            _db.Requestbusinesses.Add(requestbusiness);
            _db.SaveChanges();
        }
    }
}