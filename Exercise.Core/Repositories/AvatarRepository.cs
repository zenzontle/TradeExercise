using Exercise.Core.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Exercise.Core.Repositories
{
    public class AvatarRepository : IAvatarRepository
    {
        public Task<List<Avatar>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<Avatar> Get(int avatarId)
        {
            using (var context = new TradePMREntities())
            {
                var avatar = await context.Avatars.FindAsync(avatarId);
                return avatar;
            }
        }

        public async Task<Avatar> Create(int userId, Avatar avatar)
        {
            using (var context = new TradePMREntities())
            {
                var user = await context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new ApplicationException($"User with id {userId} not found");
                }
                if (user.AvatarId != null)
                {
                    throw new ApplicationException("User already has an avatar");
                }
                avatar.Users.Add(user);
                var avatarCreated = context.Avatars.Add(avatar);
                var created = await context.SaveChangesAsync();
                if (created == 0)
                {
                    return null;
                }
                return avatarCreated;
            }
        }

        public async Task<Avatar> Update(int userId, Avatar avatar)
        {
            using (var context = new TradePMREntities())
            {
                var user = await context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new ApplicationException($"User with id {userId} not found");
                }

                if (user.AvatarId == null)
                {
                    avatar.Users.Add(user);
                    context.Avatars.Add(avatar);
                }
                else
                {
                    var result = await context.Avatars.FindAsync(user.AvatarId);
                    if (result == null)
                    {
                        return null;
                    }
                    context.Entry(result).CurrentValues.SetValues(avatar);
                    context.Entry(result).State = EntityState.Modified;
                }

                var updated = await context.SaveChangesAsync();
                if (updated == 0)
                {
                    return null;
                }
                return avatar;
            }
        }

        public async Task<bool> Delete(int avatarId)
        {
            using (var context = new TradePMREntities())
            {
                var result = await context.Avatars.FindAsync(avatarId);
                if (result == null)
                {
                    return false;
                }
                foreach (var user in result.Users)
                {
                    context.Entry(user).Entity.AvatarId = null;
                }
                context.Entry(result).State = EntityState.Deleted;
                var deleted = await context.SaveChangesAsync();
                if (deleted == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}