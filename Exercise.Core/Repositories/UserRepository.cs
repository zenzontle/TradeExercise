using Exercise.Core.Entity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Exercise.Core.Repositories
{
    //TODO Should this throw exceptions?
    public class UserRepository : IUserRepository
    {
        public async Task<List<User>> GetAll()
        {
            using (var context = new TradePMREntities())
            {
                var users = await context.Users.ToListAsync();
                return users;
            }
        }

        public async Task<User> Get(int userId)
        {
            using (var context = new TradePMREntities())
            {
                var user = await context.Users.FindAsync(userId);
                return user;
            }
        }

        public async Task<User> Create(User user)
        {
            using (var context = new TradePMREntities())
            {
                var userCreated = context.Users.Add(user);
                var created = await context.SaveChangesAsync();
                if (created == 0)
                {
                    return null;
                }
                return userCreated;
            }
        }

        public async Task<User> Update(User user)
        {
            using (var context = new TradePMREntities())
            {
                var result = await context.Users.FindAsync(user.Id);
                if (result == null)
                {
                    return null;
                }
                context.Entry(result).CurrentValues.SetValues(user);
                context.Entry(result).State = EntityState.Modified;
                var updated = await context.SaveChangesAsync();
                if (updated == 0)
                {
                    return null;
                }
                return user;
            }
        }

        public async Task<bool> Delete(User user)
        {
            using (var context = new TradePMREntities())
            {
                var result = await context.Users.FindAsync(user.Id);
                if (result == null)
                {
                    return false;
                }
                if (result.Avatar != null)
                {
                    context.Entry(result.Avatar).State = EntityState.Deleted;
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