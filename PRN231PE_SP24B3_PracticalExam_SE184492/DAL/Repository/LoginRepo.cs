using DAL.Interface;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class LoginRepo : ILoginRepo
    {
        private readonly Prn231Sp24AlbumDbContext _context;

        public LoginRepo(Prn231Sp24AlbumDbContext context)
        {
            _context = context;
        }

        public UserRole GetAccount(string u, string p)
        {
            return _context.UserRoles.FirstOrDefault(
                a => a.Username == u && a.Passphrase == p
            );
        }
    }
}
