using Exercise.Core.Entity;
using System.Threading.Tasks;

namespace Exercise.Core.Client.Services
{
    public interface IAvatarDataService
    {
        Task<string> GetAvatar(int avatarId);

        Task<Avatar> CreateAvatar(int userId, string userAvatar);

        Task<Avatar> UpdateAvatar(int userId, string userAvatar);

        Task<bool> DeleteAvatar(int avatarId);
    }
}