using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Area
    {
        public int Id { get; set; }

        [MaxLength(25)]
        public required string Name { get; set; }
        public ICollection<ApprovalRule> ApprovalRules { get; set; } = new List<ApprovalRule>();
        public ICollection<ProjectProposal> ProjectProposals { get; set; } = new List<ProjectProposal>();
    }
}
