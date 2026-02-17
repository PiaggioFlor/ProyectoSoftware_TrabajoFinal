using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.Interfaces.Querys;
using Domain.Entities;
using Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace Infraestructure.Querys
{
    public class ProjectProposalQuery : IProjectProposalQuery
    {
        private readonly AppDbContext _context;

        public ProjectProposalQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectProposal?> GetProjectProposalId(Guid projectProposalId)
        {
            return await _context.ProjectProposals
                .Include(p => p.Areas)
                .Include(p => p.ApprovalStatus)
                .Include(p => p.ProjectType)
                .Include(p => p.User)
                    .ThenInclude(u => u.ApproverRole)
                .Include(p => p.ProjectApprovalSteps)
                    .ThenInclude(s => s.User)
                        .ThenInclude(u => u.ApproverRole)
                .Include(p => p.ProjectApprovalSteps)
                    .ThenInclude(s => s.ApproverRole)
                .Include(p => p.ProjectApprovalSteps)
                    .ThenInclude(s => s.ApprovalStatus)
                .FirstOrDefaultAsync(p => p.Id == projectProposalId);
        }

        public async Task<ICollection<ProjectProposal>> GetProjectProposals(
            string? title = null,
            int? statusId = null,
            int? createdByUserId = null,
            int? approverUserId = null)
        {
            var query = _context.ProjectProposals
                .Include(p => p.Areas)
                .Include(p => p.ProjectType)
                .Include(p => p.ApprovalStatus)
                .Include(p => p.User)
                .Include(p => p.ProjectApprovalSteps)
                    .ThenInclude(step => step.ApprovalStatus)
                .Include(p => p.ProjectApprovalSteps)
                    .ThenInclude(step => step.ApproverRole)
                        .ThenInclude(role => role.Users)
                .Include(p => p.ProjectApprovalSteps)
                    .ThenInclude(step => step.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(p => p.Title.Contains(title));

            if (statusId.HasValue)
                query = query.Where(p => p.Status == statusId.Value);

            if (createdByUserId.HasValue)
                query = query.Where(p => p.CreateBy == createdByUserId.Value);

            if(approverUserId.HasValue)
                query = query.Where(p => p.ProjectApprovalSteps
                    .Any(step =>
                        step.ApproverRole.Users.Any(u => u.Id == approverUserId.Value)
                        || (step.User != null && step.User.Id == approverUserId.Value)
                    ));

            return await query.ToListAsync();
        }

    }
}