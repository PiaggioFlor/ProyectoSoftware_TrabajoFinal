using Aplication.Dtos.Responses;
using Aplication.Interfaces.Querys;
using Aplication.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.UseCase
{
    public class AreaService : IAreaService
    {
        private readonly IAreaQuery _areaQuery;

        public AreaService(IAreaQuery areaQuery)
        {
            _areaQuery = areaQuery;
        }

        public async Task<ICollection<GenericResponse>> GetAreas()
        {
            var status = await _areaQuery.GetAreas();

            return status.Select(s => new GenericResponse
            {
                Id = s.Id,
                Name = s.Name

            }).ToList();
        }
    }
}
