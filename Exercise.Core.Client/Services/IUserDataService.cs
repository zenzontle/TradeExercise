using Exercise.Core.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Exercise.Core.Client.Services
{
    public interface IUserDataService
    {
        Task<IEnumerable<User>> GetUsers();

        Task<User> CreateUser(User user);

        Task<User> UpdateUser(User user);

        Task<bool> DeleteUser(User user);
    }
}