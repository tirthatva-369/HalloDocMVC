using BusinessLogic.Interfaces;
using DataAccess.DataContext;
using DataAccess.DataModels;
using DataAccess.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class PatientService
    {
        private readonly ApplicationDbContext _db;

        public PatientService(ApplicationDbContext db)
        {
            _db = db;
        }
        public void AddPatientInfo(PatientRequestModel PatientRequestModel)
        {
            Request request = new Request();
            request.Requesttypeid = 2;
            request.Status = 1;
            request.Createddate = DateTime.Now;
            request.Isurgentemailsent = new BitArray(1);
            request.Firstname = PatientRequestModel.first_name;
            request.Lastname = PatientRequestModel.last_name;
            request.Phonenumber = PatientRequestModel.phone;
            request.Email = PatientRequestModel.email;

            _db.Requests.Add(request);
            _db.SaveChanges();

            Requestclient info = new Requestclient();
            info.Requestid = request.Requestid;
            info.Notes = PatientRequestModel.symptoms;
            info.Firstname = PatientRequestModel.first_name;
            info.Lastname = PatientRequestModel.last_name;
            info.Phonenumber = PatientRequestModel.phone;
            info.Email = PatientRequestModel.email;
            info.Street = PatientRequestModel.street;
            info.City = PatientRequestModel.city;
            info.State = PatientRequestModel.state;
            info.Zipcode = PatientRequestModel.zip_code;

            var user = _db.Aspnetusers.Where(x => x.Id == "1").FirstOrDefault();

            User u = new();
            u.Aspnetuserid = user.Id;
            u.Firstname = PatientRequestModel.first_name;
            u.Lastname = PatientRequestModel.last_name;
            u.Email = PatientRequestModel.email;
            u.Mobile = PatientRequestModel.phone;
            u.Street = PatientRequestModel.street;
            u.City = PatientRequestModel.city;
            u.State = PatientRequestModel.state;
            u.Zipcode = PatientRequestModel.zip_code;
            u.Createdby = user.Username;
            u.Createddate = DateTime.Now;

            _db.Users.Add(u);
            _db.SaveChanges();

            _db.Requestclients.Add(info);
            _db.SaveChanges();
        }
    }
}
