using Exercise.Core.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Exercise.Core.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAll();

        Task<User> Get(int userId);

        Task<User> Create(User user);

        Task<User> Update(User user);

        Task<bool> Delete(User user);
    }
}