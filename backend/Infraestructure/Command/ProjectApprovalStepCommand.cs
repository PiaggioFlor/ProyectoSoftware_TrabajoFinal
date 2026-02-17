using Aplication.Interfaces.Commands;
using Aplication.CustomExceptions;
using Domain.Entities;
using Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Command
{
    public class ProjectApprovalStepCommand : IProjectApprovalStepCommand
    {
        private readonly AppDbContext _context;

        public ProjectApprovalStepCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task InsertProjectApprovalStep(ProjectApprovalStep projectApprovalStep)
        {
            _context.Add(projectApprovalStep);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProjectApprovalStep(ProjectApprovalStep projectApprovalStep)
        {
            _context.ProjectApprovalSteps.Update(projectApprovalStep);

            // Actualiza estado de la propuesta
            await VerificarEstadoDePropuesta(projectApprovalStep.ProjectProposalId);
        }

        public async Task VerificarEstadoDePropuesta(Guid projectProposalId)
        {
            var proposal = await _context.ProjectProposals
                .Include(p => p.ProjectApprovalSteps)
                .FirstOrDefaultAsync(x => x.Id == projectProposalId);

            if (proposal == null)
                throw new NotFoundException("No se ha encontrado el proyecto solicitado");

            var steps = proposal.ProjectApprovalSteps;

            if (steps.Any(x => x.Status == 3))
                proposal.Status = 3; // rechazado
            else if (steps.Any(x => x.Status == 4))
                proposal.Status = 4; // observado
            else if (steps.All(x => x.Status == 2))
                proposal.Status = 2; // aprobado
            else
                proposal.Status = 1; // pendiente

            _context.ProjectProposals.Update(proposal);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStepsByProposalId(Guid proposalId)
        {
            var steps = await _context.ProjectApprovalSteps
                .Where(s => s.ProjectProposalId == proposalId)
                .ToListAsync();

            if (steps.Any())
            {
                _context.ProjectApprovalSteps.RemoveRange(steps);
                await _context.SaveChangesAsync();
            }
        }
    }
}
