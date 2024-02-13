using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IPatientInterface
    {
        void AddPatientInfo(PatientRequestModel PatientRequestModel);
        void AddFamilyRequest(FamilyRequestModel familyReqModel);
        void AddConciergeRequest(ConciergeRequestModel conciergeReqModel);
        void AddBusinessRequest(BusinessRequestModel businessReqModel);
    }
}