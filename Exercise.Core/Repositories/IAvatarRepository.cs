using Exercise.Core.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Exercise.Core.Repositories
{
    public interface IAvatarRepository
    {
        Task<List<Avatar>> GetAll();

        Task<Avatar> Get(int avatarId);

        Task<Avatar> Create(int userId, Avatar avatar);

        Task<Avatar> Update(int userId, Avatar avatar);

        Task<bool> Delete(int avatar);
    }
}