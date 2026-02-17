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
    public class ApprovalStatusQuery : IApprovalStatusQuery
    {
        private readonly AppDbContext _context;

        public ApprovalStatusQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApprovalStatus?> GetApprovalStatusId(int approvalStatusId)
        {
            var approvalStatus = await _context.ApprovalStatuses.FindAsync(approvalStatusId);

            return approvalStatus;
        }
        public async Task<string?> NameStatus(int approvalStatusId)
        {
            var approvalStatus = await _context.ApprovalStatuses.FindAsync(approvalStatusId);
            return approvalStatus?.Name;
        }
        public async Task<ICollection<ApprovalStatus>> GetApprovalStatus()
        {
            var approvalStatuses = await _context.ApprovalStatuses
            .ToListAsync();

            return approvalStatuses;
        }
    }
}