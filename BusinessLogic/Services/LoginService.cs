using BusinessLogic.Interfaces;
using DataAccess.DataModels;
using DataAccess.Models;
using DataAccess.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services
{
    public class LoginService : ILoginInterface
    {
        private readonly DataAccess.DataContext.ApplicationDbContext _db;

        public LoginService(DataAccess.DataContext.ApplicationDbContext db)
        {
            _db = db;
        }

        public bool EmailCheck(LoginModel loginModel)
        {
            return _db.Aspnetusers.Any(x => x.Email == loginModel.Email);
        }

        public bool PasswordCheck(LoginModel loginModel)
        {
            return _db.Aspnetusers.Any(x => x.Passwordhash == loginModel.Passwordhash);
        }
    }
}