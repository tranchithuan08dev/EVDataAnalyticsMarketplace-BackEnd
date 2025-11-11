using BLL.Interface;
using DAL.Interface;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class LoginService : ILoginService
    {

        private readonly ILoginRepo _repository;

        public LoginService(ILoginRepo repository)
        {
            _repository = repository;
        }
        public UserRole Login(string u, string p)
        {
            return _repository.GetAccount(u, p);
        }
    }
}
