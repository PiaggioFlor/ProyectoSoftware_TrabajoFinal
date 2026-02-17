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
    public class ApprovalRuleQuery : IApprovalRuleQuery
    {
        private readonly AppDbContext _context;

        public ApprovalRuleQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApprovalRule?> GetApprovalRuleId(int approvalRuleId)
        {
            var approvalRule = await _context.ApprovalRules.FindAsync(approvalRuleId);
            return approvalRule;
        }
        public async Task<List<ApprovalRule>> GetAll()
        {
            return await _context.ApprovalRules.ToListAsync();
        }

        /*  public async Task<List<ApprovalRule>> GetApprovalRuleCondiciones(decimal amount, int areaId, int typeId)
          {
              var rules = await _context.ApprovalRules
              .Where(x =>
               (x.Type == null || x.Type == typeId) &&
               (x.Area == null || x.Area == areaId) &&
               (amount >= x.MinAmount && (x.MaxAmount == 0 || amount <= x.MaxAmount))
               )
              .GroupBy(x => x.StepOrder)
              .Select(g => g
              .OrderByDescending(x =>
                  (x.Type != null ? 1 : 0) +
                  (x.Area != null ? 1 : 0) +
                  (x.MaxAmount != 0 ? 1 : 0)
                  ).First()
                  )
                  .ToListAsync();
              return rules;
          }*/
    }
}