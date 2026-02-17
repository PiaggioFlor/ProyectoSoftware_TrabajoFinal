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
    public class ApproverRoleService : IApproverRoleService
    {
        private readonly IApproverRoleQuery _roleQuery;

        public ApproverRoleService(IApproverRoleQuery roleQuery)
        {
            _roleQuery = roleQuery;
        }

        public async Task<ICollection<GenericResponse>> GetApproverRoles()
        {
            var status = await _roleQuery.GetApproverRoles();

            return status.Select(s => new GenericResponse
            {
                Id = s.Id,
                Name = s.Name

            }).ToList();
        }
    }
}
