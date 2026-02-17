using Aplication.Dtos.Requests;
using Aplication.Dtos.Responses;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Services
{
    public interface IProjectProposalService
    {
        Task<ProjectResponse> CreateAsync(CreateProjectRequest request);
        Task<ProjectResponse> UpdateProjectAsync(Guid proposalId, UpdateProjectRequest request);
        Task<ProjectResponse?> GetProjectProposalByIdAsync(Guid id);
        Task<ICollection<ProjectResponse>> GetProjectProposals(string? title, int? status, int? applicant, int? approvalUser);
        Task<ProjectResponse> ApprovalAction(Guid projectProposalId, UpdateStepRequest request);

    }
}
