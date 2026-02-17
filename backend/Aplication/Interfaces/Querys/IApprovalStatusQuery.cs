using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Querys
{
    public interface IApprovalStatusQuery
    {
        Task<ApprovalStatus?> GetApprovalStatusId(int approvalStatusId);
        Task<string?> NameStatus(int approvalStatusId);
        Task<ICollection<ApprovalStatus>> GetApprovalStatus();
    }
}

