using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Dtos.Responses
{
    public class UsersResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public GenericResponse ApproverRole { get; set; }
    }
}
