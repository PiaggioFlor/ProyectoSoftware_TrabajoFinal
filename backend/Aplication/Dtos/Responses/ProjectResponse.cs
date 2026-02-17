using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Dtos.Responses
{
    public class ProjectResponse
    {
        public Guid Id { get; set; }
        public string Title {  get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int Duration { get; set; }
        public UsersResponse User { get; set; }
        public GenericResponse Area { get; set; }
        public GenericResponse Status { get; set; }
        public GenericResponse Type { get; set; }
        public List<ApprovalStepResponse>? Steps { get; set; }

        
    }

}
