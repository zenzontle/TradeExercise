using Exercise.Core.Entity;
using Exercise.Core.Repositories;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Exercise.API.Controllers
{
    [RoutePrefix("api/avatar")]
    public class AvatarController : ApiController
    {
        private readonly IAvatarRepository _avatarRepository;
        private readonly IUserRepository _userRepository;

        public AvatarController(IAvatarRepository avatarRepository, IUserRepository userRepository)
        {
            _avatarRepository = avatarRepository;
            _userRepository = userRepository;
        }

        [HttpGet, Route("get")]
        public async Task<IHttpActionResult> Get(int avatarId)
        {
            try
            {
                if (avatarId == 0)
                {
                    return BadRequest($"Avatar id {avatarId} is not valid");
                }
                var avatar = await _avatarRepository.Get(avatarId);
                if (avatar == null)
                {
                    return BadRequest("Avatar not found");
                }
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(avatar.Image)
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                var response = ResponseMessage(result);
                return response;
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

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> Create()
        {
            try
            {
                var userIdString = HttpContext.Current.Request.Form.Get("userId");
                if (string.IsNullOrWhiteSpace(userIdString))
                {
                    return BadRequest("User id is empty");
                }
                if (!int.TryParse(userIdString, out int userId))
                {
                    return BadRequest("User id is not a valid number");
                }
                var avatar = HttpContext.Current.Request.Files.Get("avatar");
                if (avatar == null)
                {
                    return BadRequest("Avatar is empty");
                }
                if (avatar.ContentLength > 256000)
                {
                    return BadRequest("Avatar size exceeds 256K");
                }
                if (!avatar.FileName.EndsWith(".jpg"))
                {
                    return BadRequest("Only .jpg extension is supported");
                }
                //TODO Check extension (only jpgs)

                byte[] avatarBytes;
                using (var ms = new MemoryStream())
                {
                    avatar.InputStream.CopyTo(ms);
                    avatarBytes = ms.ToArray();
                }

                var newAvatar = new Avatar
                {
                    Image = avatarBytes
                };

                var avatarCreated = await _avatarRepository.Create(userId, newAvatar);
                if (avatarCreated == null)
                {
                    return BadRequest("Could not create avatar");
                }
                return Ok(avatarCreated);
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
        public async Task<IHttpActionResult> Update()
        {
            try
            {
                var userIdString = HttpContext.Current.Request.Form.Get("userId");
                if (string.IsNullOrWhiteSpace(userIdString))
                {
                    return BadRequest("User id is empty");
                }
                if (!int.TryParse(userIdString, out int userId))
                {
                    return BadRequest("User id is not a valid number");
                }

                var newAvatar = new Avatar();

                var avatar = HttpContext.Current.Request.Files.Get("avatar");
                if (avatar != null)
                {
                    if (avatar.ContentLength > 256000)
                    {
                        return BadRequest("Avatar size exceeds 256K");
                    }
                    if (!avatar.FileName.EndsWith(".jpg"))
                    {
                        return BadRequest("Only .jpg extension is supported");
                    }

                    byte[] avatarBytes;
                    using (var ms = new MemoryStream())
                    {
                        avatar.InputStream.CopyTo(ms);
                        avatarBytes = ms.ToArray();
                    }
                    newAvatar.Image = avatarBytes;
                }

                var avatarCreated = await _avatarRepository.Update(userId, newAvatar);
                if (avatarCreated == null)
                {
                    return BadRequest("Could not create avatar");
                }
                return Ok(avatarCreated);
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
        public async Task<IHttpActionResult> Delete()
        {
            try
            {
                var avatarIdString = HttpContext.Current.Request.Form.Get("avatarId");
                if (string.IsNullOrWhiteSpace(avatarIdString))
                {
                    return BadRequest("Avatar id is empty");
                }
                if (!int.TryParse(avatarIdString, out int avatarId))
                {
                    return BadRequest("Avatar id is not a valid number");
                }
                if (avatarId == 0)
                {
                    return BadRequest("Avatar provided is empty");
                }

                var deleted = await _avatarRepository.Delete(avatarId);
                if (!deleted)
                {
                    return BadRequest($"Avatar with id {avatarId} couldn't be deleted");
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