using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;

namespace Huawei_Commissioning_App.Classes.Services
{
    public class FirebaseTemplateService
    {
        private readonly string _bucketName;
        private readonly HttpClient _httpClient;

        public FirebaseTemplateService(string bucketName)
        {
            _bucketName = bucketName;
            _httpClient = new HttpClient();
        }

        // Downloads reference template from Firebase Storage directly to memory (RAM)
        public List<string> DownloadTemplate(string templateName)
        {
            try
            {
                // URL Format: https://firebasestorage.googleapis.com/v0/b/<bucket>/o/templates%2F<filename>?alt=media
                string url = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/templates%2F{templateName}?alt=media";
                var response = _httpClient.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to download template. Status Code: {response.StatusCode}");
                }

                string content = response.Content.ReadAsStringAsync().Result;
                
                // Split by line breaks to return a list of lines
                return new List<string>(content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching cloud template '{templateName}': {ex.Message}");
                throw;
            }
        }

        // Uploads a new or replacement template to Firebase Storage (Admins Only)
        public bool UploadTemplate(string templateName, string content)
        {
            try
            {
                // URL Format: https://firebasestorage.googleapis.com/v0/b/<bucket>/o?name=templates%2F<filename>
                string url = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o?name=templates%2F{templateName}";
                var httpContent = new StringContent(content, System.Text.Encoding.UTF8, "text/plain");

                var response = _httpClient.PostAsync(url, httpContent).Result;
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload Template Error: {ex.Message}");
                return false;
            }
        }

        // Deletes a template from Firebase Storage (Admins Only)
        public bool DeleteTemplate(string templateName)
        {
            try
            {
                // URL Format: https://firebasestorage.googleapis.com/v0/b/<bucket>/o/templates%2F<filename>
                string url = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/templates%2F{templateName}";
                var response = _httpClient.DeleteAsync(url).Result;
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Template Error: {ex.Message}");
                return false;
            }
        }

        // Lists all template names currently stored in the Firebase Storage templates/ folder (Admins Only)
        public List<string> ListTemplates()
        {
            var list = new List<string>();
            try
            {
                // URL: https://firebasestorage.googleapis.com/v0/b/<bucket>/o?prefix=templates%2F
                string url = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o?prefix=templates%2F";
                var response = _httpClient.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode) return list;

                string json = response.Content.ReadAsStringAsync().Result;
                using (var doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("items", out var itemsElement) && itemsElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in itemsElement.EnumerateArray())
                        {
                            if (item.TryGetProperty("name", out var nameElement))
                            {
                                string? fullName = nameElement.GetString();
                                if (!string.IsNullOrEmpty(fullName))
                                {
                                    // Remove the "templates/" prefix
                                    string fileName = fullName.Replace("templates/", "");
                                    if (!string.IsNullOrEmpty(fileName))
                                    {
                                        list.Add(fileName);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"List Templates Error: {ex.Message}");
            }
            return list;
        }
    }
}
