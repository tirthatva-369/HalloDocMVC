using Microsoft.AspNetCore.Http;

namespace DataAccess.Models
{
    public class PatientRequestModel
    {
        public string symptoms { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateOnly dateOfBirth { get; set; }
        public string email { get; set; }
        public string phoneNo { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipCode { get; set; }
        public int roomSuite { get; set; }
        public List<IFormFile> file { get; set; }

    }

    public class FamilyRequestModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phoneNo { get; set; }
        public string relation { get; set; }
        public string symptoms { get; set; }
        public string patientFirstName { get; set; }
        public string patientLastName { get; set; }
        public DateOnly patientDob { get; set; }
        public string patientEmail { get; set; }
        public string patientPhoneNo { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipCode { get; set; }
        public int roomSuite { get; set; }
    }

    public class ConciergeRequestModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phoneNo { get; set; }
        public string hotelName { get; set; }
        public string symptoms { get; set; }
        public string patientFirstName { get; set; }
        public string patientLastName { get; set; }
        public DateOnly patientDob { get; set; }
        public string patientEmail { get; set; }
        public string patientPhoneNo { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipCode { get; set; }
        public int roomSuite { get; set; }
    }

    public class BusinessRequestModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phoneNo { get; set; }
        public string businessName { get; set; }
        public string caseNo { get; set; }
        public string symptoms { get; set; }
        public string patientFirstName { get; set; }
        public string patientLastName { get; set; }
        public DateOnly patientDob { get; set; }
        public string patientEmail { get; set; }
        public string patientPhoneNo { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipCode { get; set; }
        public int roomSuite { get; set; }
    }

    public class PatientDashboard
    {
        public DateTime createdDate { get; set; }
        public string currentStatus { get; set; }
        public string document { get; set; }
    }

    public class PatientDashboardInfo
    {
        public List<PatientDashboard> patientDashboardItems { get; set; }
    }

    public class MedicalHistory
    {
        public DateTime createdDate { get; set; }
        public string currentStatus { get; set; }
        public List<string> document { get; set; }
    }
    public class MedicalHistoryList
    {
        public List<MedicalHistory> medicalHistoriesList { get; set; }
    }
}