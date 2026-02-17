using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Dtos.Responses
{
    public class ProjectShortResponse
    {
        public Guid id {  get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public decimal amount { get; set; }
        public int duration { get; set; }
        public string area { get; set; }
        public string status { get; set; }
        public string type { get; set; }

    }
}
