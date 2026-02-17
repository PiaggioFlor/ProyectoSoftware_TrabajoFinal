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
    public class AreaQuery : IAreaQuery
    {
        private readonly AppDbContext _context;

        public AreaQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Area?> GetAreaId(int areaId)
        {
            var area = await _context.Areas.FindAsync(areaId);

            return area;
        }
        public async Task<ICollection<Area>> GetAreas()
        {
            var areas = await _context.Areas
            .ToListAsync();

            return areas;
        }
    }
}
