using Aplication.Dtos.Responses;
using Aplication.Interfaces.Querys;
using Aplication.Interfaces.Services;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.UseCase
{
    public class ApprovalStatusService : IApprovalStatusService
    {
        private readonly IApprovalStatusQuery _statusQuery;

        public ApprovalStatusService(IApprovalStatusQuery statusQuery)
        {
            _statusQuery = statusQuery;
        }

        public async Task<ICollection<GenericResponse>> GetApprovalStatus()
        {
            var status = await _statusQuery.GetApprovalStatus();

            return status.Select(s => new GenericResponse
            {
                Id = s.Id,
                Name = s.Name

            }).ToList();
        }
    }
}
