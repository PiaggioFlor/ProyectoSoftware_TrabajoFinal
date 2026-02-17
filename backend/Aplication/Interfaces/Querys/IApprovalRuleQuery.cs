using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Querys
{
    public interface IApprovalRuleQuery
    {
        Task<ApprovalRule?> GetApprovalRuleId(int approvalRuleId);
        Task<List<ApprovalRule>> GetAll();
        /*Task<List<ApprovalRule>> GetApprovalRuleCondiciones(decimal amount, int areaId, int typeId);*/
    }
}
