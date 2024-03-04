using BusinessLogic.Interfaces;
using DataAccess.DataContext;
using DataAccess.DataModels;
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
            var check = _db.Aspnetusers.FirstOrDefault(x => x.Email == loginModel.Email);
            if (check.Passwordhash == loginModel.Passwordhash) { return true; }
            else { return false; }
        }

        public User Login(LoginModel loginModel)
        {
            var obj = _db.Aspnetusers.ToList();
            User user = new User();

            foreach (var item in obj)
            {
                if (item.Email == loginModel.Email && item.Passwordhash == loginModel.Passwordhash)
                {
                    user = _db.Users.FirstOrDefault(u => u.Aspnetuserid == item.Id);
                    return user;
                }
            }
            return user;
        }
    }
}