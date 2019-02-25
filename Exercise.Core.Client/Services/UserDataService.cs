using Exercise.Core.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Exercise.Core.Client.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly HttpClient _client;

        public UserDataService()
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

        public async Task<IEnumerable<User>> GetUsers()
        {
            var response = await _client.GetAsync("api/user/getAll");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadAsAsync<IEnumerable<User>>();
        }

        public async Task<User> CreateUser(User user)
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(user, Formatting.None), Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("api/user/create", stringContent);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadAsAsync<User>();
        }

        public async Task<User> UpdateUser(User user)
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(user, Formatting.None), Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("api/user/update", stringContent);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadAsAsync<User>();
        }

        public async Task<bool> DeleteUser(User user)
        {
            var stringContent = new StringContent(JsonConvert.SerializeObject(user, Formatting.None), Encoding.UTF8,
                "application/json");
            var response = await _client.PostAsync("api/user/delete", stringContent);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            return await response.Content.ReadAsAsync<bool>();
        }
    }
}