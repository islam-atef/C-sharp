using System;
using System.Net.Http;
using System.Text.Json;
using Huawei_Commissioning_App.Classes.Models;

namespace Huawei_Commissioning_App.Classes.Strategies
{
    public class FirebaseIpProviderStrategy : IIpProviderStrategy
    {
        private readonly string _databaseUrl;
        private readonly string _authSecret;
        private readonly HttpClient _httpClient;

        public FirebaseIpProviderStrategy(string databaseUrl, string authSecret)
        {
            _databaseUrl = databaseUrl.EndsWith("/") ? databaseUrl : databaseUrl + "/";
            _authSecret = authSecret;
            _httpClient = new HttpClient();
        }

        public bool GetIPs(IpPlan ipPlan, string? cabinetCode)
        {
            if (string.IsNullOrEmpty(cabinetCode)) return false;

            try
            {
                // Request URL: https://<project>.firebaseio.com/ipplans/<cabinetCode>.json?auth=<secret>
                string url = $"{_databaseUrl}ipplans/{cabinetCode}.json?auth={_authSecret}";
                var response = _httpClient.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode) return false;

                string json = response.Content.ReadAsStringAsync().Result;
                if (json == "null" || string.IsNullOrEmpty(json)) return false;

                var data = JsonSerializer.Deserialize<IpPlan>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                if (data != null)
                {
                    // Copy properties
                    ipPlan.PopName = data.PopName;
                    ipPlan.TedMgGatewayIp = data.TedMgGatewayIp;
                    ipPlan.TedMgSH1Ip = data.TedMgSH1Ip;
                    ipPlan.TedMgSH2Ip = data.TedMgSH2Ip;
                    ipPlan.MgGatewayIp = data.MgGatewayIp;
                    ipPlan.MgSH1Ip = data.MgSH1Ip;
                    ipPlan.MgSH2Ip = data.MgSH2Ip;
                    ipPlan.MgSH3Ip = data.MgSH3Ip;
                    ipPlan.SigGatewayIp = data.SigGatewayIp;
                    ipPlan.SigSH1Ip = data.SigSH1Ip;
                    ipPlan.SigSH2Ip = data.SigSH2Ip;
                    ipPlan.FvnoEmGatewayIp = data.FvnoEmGatewayIp;
                    ipPlan.FvnoEmSH1Ip = data.FvnoEmSH1Ip;
                    ipPlan.FvnoEmSH2Ip = data.FvnoEmSH2Ip;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Firebase IP Fetch Error: {ex.Message}");
            }
            return false;
        }

        // Method to dynamically upload a new cabinet IP plan (Write Access)
        public bool AddIpPlan(string cabinetCode, IpPlan ipPlan)
        {
            try
            {
                string url = $"{_databaseUrl}ipplans/{cabinetCode}.json?auth={_authSecret}";
                string json = JsonSerializer.Serialize(ipPlan);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = _httpClient.PutAsync(url, content).Result;
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Firebase IP Upload Error: {ex.Message}");
                return false;
            }
        }
    }
}
