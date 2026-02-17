using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Commands
{
    public interface IProjectApprovalStepCommand
    {
        Task InsertProjectApprovalStep(ProjectApprovalStep projectApprovalStep);
        Task UpdateProjectApprovalStep(ProjectApprovalStep projectApprovalStep);
        Task DeleteStepsByProposalId(Guid proposalId);
    }
}