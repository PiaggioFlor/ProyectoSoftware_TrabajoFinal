using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Dtos.Responses
{
    public class ApprovalStepResponse
    {
        public long Id {  get; set; }
        public int StepOrder { get; set; }
        public DateTime? DecisionDate { get; set; }
        public string? Observations { get; set; }
        public UsersResponse ApproverUser { get; set; }
        public GenericResponse ApproverRole { get; set; }
        public GenericResponse Status { get; set; }
    }
}
