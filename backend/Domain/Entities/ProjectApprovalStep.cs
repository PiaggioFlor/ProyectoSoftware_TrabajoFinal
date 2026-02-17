using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProjectApprovalStep
    {
        public long Id { get; set; }
        public Guid ProjectProposalId { get; set; }
        public int? ApproverUserId { get; set; }
        public int ApproverRoleId { get; set; }
        public int Status { get; set; }
        public int StepOrder { get; set; }
        public DateTime? DecisionDate { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string? Observations { get; set; }
        public ProjectProposal ProjectProposal { get; set; } = null!;
        public ApprovalStatus ApprovalStatus { get; set; } = null!;
        public ApproverRole ApproverRole { get; set; } = null!;
        public User? User { get; set; }
    }
}
