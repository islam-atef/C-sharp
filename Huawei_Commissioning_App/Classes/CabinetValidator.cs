using System;
using System.Net.Http;
using System.Text.Json;
using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes
{
    public class CabinetValidator
    {
        private readonly string _databaseUrl;
        private readonly string _authSecret;
        private readonly HttpClient _httpClient;

        public CabinetValidator(string databaseUrl, string authSecret)
        {
            _databaseUrl = databaseUrl.EndsWith("/") ? databaseUrl : databaseUrl + "/";
            _authSecret = authSecret;
            _httpClient = new HttpClient();
        }

        // Validates whether the cabinet itself is acceptable for commissioning
        public string Validate(CabinetInfo cabinet)
        {
            // Default implementation returns "Accepted"
            return "Accepted";
        }

        // Validates user Key against Firebase and returns access level and region details
        public UserAccessInfo? ValidateKey(string userKey)
        {
            if (string.IsNullOrEmpty(userKey)) return null;

            try
            {
                // Query: https://<project>.firebaseio.com/keys/<userKey>.json?auth=<secret>
                string url = $"{_databaseUrl}keys/{userKey}.json?auth={_authSecret}";
                var response = _httpClient.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode) return null;

                string json = response.Content.ReadAsStringAsync().Result;
                if (json == "null" || string.IsNullOrEmpty(json)) return null;

                return JsonSerializer.Deserialize<UserAccessInfo>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Key Validation Error: {ex.Message}");
                return null;
            }
        }
    }

    public class UserAccessInfo
    {
        public string? AccessLevel { get; set; } // "Read" or "Write"
        public string? Region { get; set; }      // e.g., "11", "12", "All"
    }
}
