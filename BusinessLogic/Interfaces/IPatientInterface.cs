using DataAccess.DataModels;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BusinessLogic.Interfaces
{
    public interface IPatientInterface
    {
        void AddPatientInfo(PatientRequestModel PatientRequestModel);
        void AddFamilyRequest(FamilyRequestModel familyReqModel);
        void AddConciergeRequest(ConciergeRequestModel conciergeReqModel);
        void AddBusinessRequest(BusinessRequestModel businessReqModel);
        Task<bool> IsEmailExists(string email);
        MedicalHistoryList GetMedicalHistory(int userid);
        DocumentModel GetAllDocById(int requestId);
        Profile GetProfile(int userid);
        bool EditProfile(Profile profile);
        bool UploadDocuments(List<IFormFile> files, int reqId);
    }
}