using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface ILoginInterface
    {
        public bool EmailCheck(LoginModel loginModel);
        public bool PasswordCheck(LoginModel loginModel);
    }
}