//using Aplication.Interfaces.Commands;
using Infraestructure.Persistence;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplication.Interfaces.Commands;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Command
{
    public class ProjectProposalCommand : IProjectProposalCommand
    {
        private readonly AppDbContext _context;

        public ProjectProposalCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task InsertProjectProposal(ProjectProposal projectproposal)
        {
            _context.Add(projectproposal);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateProjectProposal(ProjectProposal projectProposal)
        {
            _context.ProjectProposals.Update(projectProposal);
            await _context.SaveChangesAsync();
        }
    }
}
