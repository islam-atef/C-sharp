using System;

namespace Huawei_Commissioning_App.Classes.Models
{
    public class CabinetInfo
    {
        public string? CabinetFamilyName { get; set; } // e.g., Huawei, Nokia
        public string? CabinetType { get; set; }       // e.g., MA5818, GPON300
        public string? Code1 { get; set; }             // e.g., 11-1-38-80
        public string? Code2 { get; set; }

        // Extracted Region (first two digits of Code1, e.g., "11")
        public string Region => !string.IsNullOrEmpty(Code1) && Code1.Length >= 2 
            ? Code1.Substring(0, 2) 
            : "Unknown";
    }
}
