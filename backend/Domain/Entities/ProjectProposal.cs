using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProjectProposal
    {
        public Guid Id { get; set; }
        [MaxLength(255)]
        public required string Title { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public required string Description { get; set; }
        public int Area { get; set; } 
        public int Type { get; set; }
        public decimal EstimatedAmount { get; set; }
        public int EstimatedDuration { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public int CreateBy { get; set; }
        public Area Areas { get; set; } = null!;
        public ICollection<ProjectApprovalStep> ProjectApprovalSteps { get; set; } = new List<ProjectApprovalStep>();
        public ProjectType ProjectType { get; set; } = null!;
        public User User { get; set; } = null!;
        public ApprovalStatus ApprovalStatus { get; set; } = null!;

    }
}
