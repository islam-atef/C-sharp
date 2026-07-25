using System;
using System.Net.Http;
using System.Text.Json;

namespace Huawei_Commissioning_App.Classes.Services
{
    public class FirebaseLogService
    {
        private readonly string _databaseUrl;
        private readonly string _authSecret;
        private readonly HttpClient _httpClient;

        public FirebaseLogService(string databaseUrl, string authSecret)
        {
            _databaseUrl = databaseUrl.EndsWith("/") ? databaseUrl : databaseUrl + "/";
            _authSecret = authSecret;
            _httpClient = new HttpClient();
        }

        public void WriteLog(string userKey, string cabinetCode, string cabinetType, string status)
        {
            try
            {
                var log = new
                {
                    UserKey = userKey,
                    CabinetCode = cabinetCode,
                    CabinetType = cabinetType,
                    GeneratedAt = DateTime.UtcNow.ToString("o"),
                    Status = status
                };

                string url = $"{_databaseUrl}logs.json?auth={_authSecret}";
                string json = JsonSerializer.Serialize(log);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // Post appends a new record to the list
                _ = _httpClient.PostAsync(url, content).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logging Error: {ex.Message}");
            }
        }
    }
}
