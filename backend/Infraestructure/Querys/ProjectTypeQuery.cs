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
    public class ProjectTypeQuery : IProjectTypeQuery
    {
        private readonly AppDbContext _context;

        public ProjectTypeQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectType?> GetProjectTypeId(int typeId)
        {
            var type = await _context.ProjectTypes.FindAsync(typeId);

            return type;
        }
        public async Task<ICollection<ProjectType>> GetProjectTypes()
        {
            var projectTypes = await _context.ProjectTypes
            .ToListAsync();

            return projectTypes;
        }
    }
}
