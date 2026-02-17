using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Querys
{
    public interface IProjectTypeQuery
    {
        Task<ProjectType?> GetProjectTypeId(int typeId);
        Task<ICollection<ProjectType>> GetProjectTypes();
    }
}
