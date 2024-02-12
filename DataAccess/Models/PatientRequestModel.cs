using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class PatientRequestModel
    {
        public string symptoms { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public DateOnly dateofbirth { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip_code { get; set; }
        public string room_suite { get; set; }
        public string upload_document { get; set; }
    }
}