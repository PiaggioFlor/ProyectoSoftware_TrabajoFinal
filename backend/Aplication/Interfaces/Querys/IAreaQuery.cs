using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Querys
{
    public interface IAreaQuery
    {
        Task<Area?> GetAreaId(int areaId);
        Task<ICollection<Area>> GetAreas();
    }
}
