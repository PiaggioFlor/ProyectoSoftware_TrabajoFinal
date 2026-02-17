using Aplication.Dtos.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Services
{
    public interface IApproverRoleService
    {
        Task<ICollection<GenericResponse>> GetApproverRoles();
    }
}
