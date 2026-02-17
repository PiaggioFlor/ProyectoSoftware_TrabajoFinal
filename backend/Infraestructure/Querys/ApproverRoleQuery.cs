using Aplication.Interfaces.Querys;
using Domain.Entities;
using Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Querys
{
    public class ApproverRoleQuery : IApproverRoleQuery
    {
        private readonly AppDbContext _context;

        public ApproverRoleQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApproverRole?> GetApproverRoleId(int approverRoleId)
        {
            var approverRoles = await _context.ApproverRoles.FindAsync(approverRoleId);

            return approverRoles;
        }

        public async Task<ICollection<User>> ListaUsersporRole(int approverRoleId)
        {
            var users = await _context.Users
            .Where(x => x.Role == approverRoleId)
            .ToListAsync();

            return users;
        }

        public async Task<ICollection<ApproverRole>> GetApproverRoles()
        {
            var approverRoles = await _context.ApproverRoles
            .ToListAsync();

            return approverRoles;
        }
    }
}