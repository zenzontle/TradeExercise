using Exercise.Core.Entity;
using Exercise.Core.Repositories;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Exercise.API.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet, Route("getAll")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var users = await _userRepository.GetAll();
                return Ok(users);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
#if DEBUG
                return InternalServerError(e);
#else
                return InternalServerError();
#endif
            }
        }

        [HttpGet, Route("get")]
        public async Task<IHttpActionResult> GetU(int userId)
        {
            if (userId == 0)
            {
                return BadRequest($"User id {userId} is not valid");
            }
            var user = await _userRepository.Get(userId);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            return Ok(user);
        }

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> Create(User user)
        {
            try
            {
                //TODO Check if more extensive validations are possible 'web api controller validations'
                if (user == null)
                {
                    return BadRequest("User is empty");
                }

                var userCreated = await _userRepository.Create(user);
                if (userCreated == null)
                {
                    return BadRequest("Could not create user");
                }
                return Ok(userCreated);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
#if DEBUG
                return InternalServerError(e);
#else
                return InternalServerError();
#endif
            }
        }

        [HttpPost, Route("update")]
        public async Task<IHttpActionResult> Update(User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("User provided is empty");
                }

                var userUpdated = await _userRepository.Update(user);
                if (userUpdated == null)
                {
                    return BadRequest($"User with id {user.Id} couldn't be updated");
                }
                return Ok(userUpdated);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
#if DEBUG
                return InternalServerError(e);
#else
                return InternalServerError();
#endif
            }
        }

        [HttpPost, Route("delete")]
        public async Task<IHttpActionResult> Delete(User user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("User provided is empty");
                }

                var deleted = await _userRepository.Delete(user);
                if (!deleted)
                {
                    return BadRequest($"User with id {user.Id} couldn't be deleted");
                }
                return Ok(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
#if DEBUG
                return InternalServerError(e);
#else
                return InternalServerError();
#endif
            }
        }
    }
}