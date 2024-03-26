using DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string password { get; set; }
    }

    public class LoginResponseViewModel
    {
        public ResponseStatus Status { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
    }

    public class CreateAccountModel
    {
        [Required(ErrorMessage = "Email is required")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        public string? confirmPassword { get; set; }
    }
}
