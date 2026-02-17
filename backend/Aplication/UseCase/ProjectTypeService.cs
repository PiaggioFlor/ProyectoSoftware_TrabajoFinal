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
    public class ProjectTypeService : IProjectTypeService
    {
        private readonly IProjectTypeQuery _typeQuery;

        public ProjectTypeService(IProjectTypeQuery typeQuery)
        {
            _typeQuery = typeQuery;
        }

        public async Task<ICollection<Dtos.Responses.GenericResponse>> GetProjectTypes()
        {
            var status = await _typeQuery.GetProjectTypes();

            return status.Select(s => new GenericResponse
            {
                Id = s.Id,
                Name = s.Name

            }).ToList();
        }
    }
}
