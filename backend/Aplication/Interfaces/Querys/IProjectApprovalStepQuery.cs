using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Querys
{
    public interface IProjectApprovalStepQuery
    {
        Task<ProjectApprovalStep?> GetProjectApprovalStepId(long projectApprovalStepId);
        Task<ProjectApprovalStep?> GetProjectApprovalStepPorProposal(Guid projectProposalId);
        Task<ICollection<ProjectApprovalStep>> ProjectProposalPending();
        Task<ICollection<ProjectApprovalStep>> GetStepsProposalId(Guid proposalId);
        Task<List<ProjectApprovalStep>> CalculateSteps(ProjectProposal project);
    }
}
