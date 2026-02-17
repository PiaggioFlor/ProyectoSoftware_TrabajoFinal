using Aplication.Interfaces.Querys;
using Domain.Entities;
using Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Querys
{
    public class UserQuery : IUserQuery
    {

        private readonly AppDbContext _context;

        public UserQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserId(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            return user;
        }

        public async Task<int?> GetRoleUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.Role;
        }

        public async Task<ICollection<User>> GetUsers()
        {
            var users = await _context.Users
                .Include(s => s.ApproverRole)
            .ToListAsync();

            return users;
        }
    }
}
