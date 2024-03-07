using DataAccess.DataModels;
using DataAccess.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface IAdminInterface
    {
        Aspnetuser GetAspnetuser(string email);
        StatusCountModel GetStatusCount();
        List<AdminDashTableModel> GetRequestsByStatus(int tabNo);
        ViewCaseViewModel ViewCase(int reqClientId, int RequestTypeId);
        ViewNotesModel ViewNotes(int reqClientId);
        bool UpdateAdminNotes(string AdminNotes, int RequestId);
        CancelCaseModel CancelCase(int reqId);
        bool SubmitCancelCase(CancelCaseModel cancelCaseModel);
        AssignCaseModel AssignCase(int reqId);
        List<Physician> GetPhysicianByRegion(int Regionid);
        bool SubmitAssignCase(AssignCaseModel assignCaseModel);
        BlockCaseModel BlockCase(int reqId);
        bool SubmitBlockCase(BlockCaseModel blockCaseModel);
        ViewUploadModel GetAllDocById(int requestId);
        bool UploadFiles(List<IFormFile> files, int reqId);
        bool DeleteFileById(int reqFileId);
        bool DeleteAllFiles(List<string> filename, int reqId);
        public OrdersModel OrderDetails();
    }
}