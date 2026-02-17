using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Commands
{
    public interface IProjectProposalCommand
    {
        Task InsertProjectProposal(ProjectProposal projectproposal);
        Task UpdateProjectProposal(ProjectProposal projectProposal);
    }
}