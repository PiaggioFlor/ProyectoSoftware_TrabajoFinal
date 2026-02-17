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
    public class ProjectApprovalStepQuery : IProjectApprovalStepQuery
    {
        private readonly AppDbContext _context;
        private readonly IApprovalRuleQuery _approvalRuleQuery;

        public ProjectApprovalStepQuery(AppDbContext context, IApprovalRuleQuery approvalRuleQuery)
        {
            _context = context;
            _approvalRuleQuery = approvalRuleQuery;
        }
        public async Task<ICollection<ProjectApprovalStep>> GetStepsProposalId(Guid proposalId)
        {
            return await _context.ProjectApprovalSteps
                .Where(s => s.ProjectProposalId == proposalId)
                .Include(s => s.User)
                    .ThenInclude(u => u.ApproverRole)
                .Include(s => s.ApproverRole)
                .Include(s => s.ApprovalStatus)
                .ToListAsync();
        }
        public async Task<ProjectApprovalStep?> GetProjectApprovalStepPorProposal(Guid projectProposalId)
        {
            var projectApprovalStep = await _context.ProjectApprovalSteps
                .Include(s => s.User)
                    .ThenInclude(u => u.ApproverRole)
                .Include(s => s.ApproverRole)
                .Include(s => s.ApprovalStatus)
                .FirstOrDefaultAsync(x => x.ProjectProposalId == projectProposalId);


            return projectApprovalStep;
        }
        public async Task<ICollection<ProjectApprovalStep>> ProjectProposalPending()
        {
            var approvalSteps = await _context.ProjectApprovalSteps
                .Include(s => s.ProjectProposal)
                .Where(s => (s.Status == 1 || s.Status == 4) && s.ProjectProposal.Status == 1)
                .ToListAsync();

            var result = approvalSteps
                .GroupBy(s => s.ProjectProposalId)
                .Select(g => g.OrderBy(s => s.StepOrder).First())
                .ToList();

            return result;
        }
        public async Task<ProjectApprovalStep?> GetProjectApprovalStepId(long projectApprovalStepId)
        {
            return await _context.ProjectApprovalSteps
                .Include(s => s.User)
                    .ThenInclude(u => u.ApproverRole)
                .Include(s => s.ApproverRole)
                .Include(s => s.ApprovalStatus)
                .FirstOrDefaultAsync(s => s.Id == projectApprovalStepId);
        }

        public async Task<List<ProjectApprovalStep>> CalculateSteps(ProjectProposal project)
        {
            var rules = await _approvalRuleQuery.GetAll();

            // Filtrar las reglas que aplican al monto
            var applicableRules = rules
                .Where(rule =>
                    rule.MinAmount <= project.EstimatedAmount &&
                    (rule.MaxAmount == 0 || rule.MaxAmount >= project.EstimatedAmount)
                )
                .ToList();

            // Agrupar por orden de paso
            var groupedByStep = applicableRules
                .GroupBy(rule => rule.StepOrder)
                .OrderBy(g => g.Key)
                .ToList();

            var selectedSteps = new List<ProjectApprovalStep>();

            foreach (var group in groupedByStep)
            {
                // Elegir la regla más específica:
                var selected = group
                    .OrderByDescending(r =>
                        (r.Area == project.Area ? 1 : 0) +
                        (r.Type == project.Type ? 1 : 0)
                    )
                    .ThenBy(r => r.Id) // Desempate
                    .FirstOrDefault();

                if (selected != null)
                {
                    selectedSteps.Add(new ProjectApprovalStep
                    {
                        ApproverRoleId = selected.ApproverRoleId,
                        StepOrder = selected.StepOrder,
                        Status = 1
                    });
                }
            }

            return selectedSteps;
        }

    }
}