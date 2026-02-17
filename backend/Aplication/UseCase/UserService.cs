using Aplication.Dtos.Responses;
using Aplication.Interfaces.Querys;
using Aplication.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplication.UseCase
{
    public class UserService : IUserService
    {
        private readonly IUserQuery _userQuery;

        public UserService(IUserQuery userQuery)
        {
            _userQuery = userQuery;
        }

        public async Task<ICollection<UsersResponse>> GetUsers()
        {
            var status = await _userQuery.GetUsers();

            return status.Select(s => new UsersResponse
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                ApproverRole = new GenericResponse
                {
                    Id = s.ApproverRole.Id,
                    Name = s.ApproverRole.Name
                }

            }).ToList();
        }
    }
}
