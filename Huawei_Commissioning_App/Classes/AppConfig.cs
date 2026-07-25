using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Huawei_Commissioning_App.Classes
{
    public static class AppConfig
    {
        public static string DatabaseUrl { get; private set; } = "";
        public static string AuthSecret { get; private set; } = "";
        public static string StorageBucket { get; private set; } = "";
        public static bool IsFirebaseConfigured { get; private set; } = false;

        static AppConfig()
        {
            Load();
        }

        public static void Load()
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                DatabaseUrl = config["Firebase:DatabaseUrl"] ?? "";
                AuthSecret = config["Firebase:AuthSecret"] ?? "";
                StorageBucket = config["Firebase:StorageBucket"] ?? "";

                IsFirebaseConfigured = !string.IsNullOrEmpty(DatabaseUrl) && 
                                       !DatabaseUrl.Contains("your-project-id");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading AppConfig: {ex.Message}");
            }
        }
    }
}
