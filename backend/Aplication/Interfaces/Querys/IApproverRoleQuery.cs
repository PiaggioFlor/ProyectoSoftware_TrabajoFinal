using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Querys
{
    public interface IApproverRoleQuery
    {
        Task<ApproverRole?> GetApproverRoleId(int approverRoleId);
        Task<ICollection<User>> ListaUsersporRole(int approverRoleId);
        Task<ICollection<ApproverRole>> GetApproverRoles();
    }
}
