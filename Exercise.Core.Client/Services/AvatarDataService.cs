using Exercise.Core.Entity;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Exercise.Core.Client.Services
{
    public class AvatarDataService : IAvatarDataService
    {
        private readonly HttpClient _client;

        public AvatarDataService()
        {
            var baseUrl = ConfigurationManager.AppSettings["TradePMRAPI"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ApplicationException("Connection string for API not found");
            }
            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> GetAvatar(int avatarId)
        {
            VerifyApplicationTempDirectoryExists();
            var filePath = Path.Combine(Path.GetTempPath(), "TradeMPRCache", $"{avatarId}.jpg");
            if (File.Exists(filePath))
            {
                return filePath;
            }
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["avatarId"] = avatarId.ToString();

            var response = await _client.GetAsync($"api/avatar/get?{query}");
            if (response.IsSuccessStatusCode)
            {
                using (var imageStream = await response.Content.ReadAsStreamAsync())
                {
                    using (var fileStream = File.Create(filePath))
                    {
                        await imageStream.CopyToAsync(fileStream);
                        fileStream.Close();
                        return filePath;
                    }
                }
            }
            return null;
        }

        public async Task<Avatar> CreateAvatar(int userId, string userAvatar)
        {
            var fullContent = new MultipartFormDataContent();
            var stringContent = new StringContent(JsonConvert.SerializeObject(userId, Formatting.None));
            var fileStream = File.ReadAllBytes(userAvatar);
            fullContent.Add(new ByteArrayContent(fileStream), "avatar", userAvatar);
            fullContent.Add(stringContent, "userId");
            var response = await _client.PostAsync("api/avatar/create", fullContent);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadAsAsync<Avatar>();
        }

        public async Task<Avatar> UpdateAvatar(int userId, string userAvatar)
        {
            var fullContent = new MultipartFormDataContent();
            var stringContent = new StringContent(JsonConvert.SerializeObject(userId, Formatting.None));
            var fileStream = File.ReadAllBytes(userAvatar);
            fullContent.Add(new ByteArrayContent(fileStream), "avatar", userAvatar);
            fullContent.Add(stringContent, "userId");
            var response = await _client.PostAsync("api/avatar/update", fullContent);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadAsAsync<Avatar>();
        }

        public async Task<bool> DeleteAvatar(int avatarId)
        {
            var fullContent = new MultipartFormDataContent();
            var stringContent = new StringContent(JsonConvert.SerializeObject(avatarId, Formatting.None));
            fullContent.Add(stringContent, "avatarId");
            var response = await _client.PostAsync("api/avatar/delete", fullContent);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            return await response.Content.ReadAsAsync<bool>();
        }

        private void VerifyApplicationTempDirectoryExists()
        {
            var directoryPath = Path.Combine(Path.GetTempPath(), "TradeMPRCache");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}