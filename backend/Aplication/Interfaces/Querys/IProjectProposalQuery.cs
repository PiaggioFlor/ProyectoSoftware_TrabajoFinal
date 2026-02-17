using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Querys
{
    public interface IProjectProposalQuery
    {
        Task<ProjectProposal?> GetProjectProposalId(Guid projectProposalId);
        Task<ICollection<ProjectProposal>> GetProjectProposals(
            string? title = null,
            int? statusId = null,
            int? createdByUserId = null,
            int? approverUserId = null);
    }
}

