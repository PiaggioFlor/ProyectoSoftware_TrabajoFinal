using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.Interfaces.Querys
{
    public interface IUserQuery
    {
        Task<User?> GetUserId(int userId);
        Task<int?> GetRoleUser(int userId);
        Task<ICollection<User>> GetUsers();
    }
}
