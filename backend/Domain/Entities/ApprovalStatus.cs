using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ApprovalStatus
    {
        public int Id { get; set; }

        [MaxLength(25)]
        public required string Name { get; set; }
        public ICollection<ProjectApprovalStep> ProjectApprovalSteps { get; set; } = new List<ProjectApprovalStep>();
        public ICollection<ProjectProposal> ProjectProposals { get; set; } = new List<ProjectProposal>();
    }
}
