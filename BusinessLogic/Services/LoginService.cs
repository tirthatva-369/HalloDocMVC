using BusinessLogic.Interfaces;
using DataAccess.DataContext;
using DataAccess.Models;

namespace BusinessLogic.Services
{
    public class LoginService : ILoginInterface
    {
        private readonly ApplicationDbContext _db;

        public LoginService(ApplicationDbContext db)
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