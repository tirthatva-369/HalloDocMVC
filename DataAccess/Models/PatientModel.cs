﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string uploadDocument { get; set; }
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
}