using System;
using System.Collections.Generic;
using System.IO;
using Huawei_Commissioning_App.Classes.Models;
using Huawei_Commissioning_App.Classes.Modifiers;
using Huawei_Commissioning_App.Classes.Strategies;
using Huawei_Commissioning_App.Classes.Services;

namespace Huawei_Commissioning_App.Classes
{
    public class CommissioningEngine
    {
        private readonly List<IConfigModifier> _modifiers;
        private readonly OperationalContext _context;
        private readonly IIpProviderStrategy _ipProvider;
        private readonly FirebaseTemplateService _templateService;
        private readonly bool _useCloudTemplates;

        public CommissioningEngine(OperationalContext context, IIpProviderStrategy ipProvider, FirebaseTemplateService templateService, bool useCloudTemplates)
        {
            _context = context;
            _ipProvider = ipProvider;
            _templateService = templateService;
            _useCloudTemplates = useCloudTemplates;
            _modifiers = new List<IConfigModifier>
            {
                new IpConfigModifier(),
                new NamingConfigModifier(),
                new SnmpConfigModifier(),
                new LinkAggregationModifier(),
                new PortConfigModifier()
            };
        }

        public bool ProcessCabinet(CabinetInfo cabinet)
        {
            try
            {
                int processCount = GetProcessCount(cabinet);
                int processCounter = 0;
                
                var ipPlan = new IpPlan();

                do
                {
                    string? currentCode = cabinet.Code1;
                    
                    if (cabinet.CabinetFamilyName == "Huawei")
                    {
                        if (processCounter == 0)
                        {
                            // Get all needed Data for Code1
                            _ipProvider.GetIPs(ipPlan, cabinet.Code1);
                        }
                        else if (processCounter == 2)
                        {
                            // Get all needed Data for Code2
                            _ipProvider.GetIPs(ipPlan, cabinet.Code2);
                            currentCode = cabinet.Code2;
                        }
                        else if (processCounter == 1)
                        {
                            currentCode = cabinet.Code1;
                        }
                        else if (processCounter == 3)
                        {
                            currentCode = cabinet.Code2;
                        }
                    }
                    else if (cabinet.CabinetFamilyName == "Nokia")
                    {
                        _ipProvider.GetIPs(ipPlan, cabinet.Code1);
                    }

                    // Get Reference File Path
                    string referencePath = GetReferenceFilePath(cabinet, processCounter);
                    
                    // Set Output Folder and File Path
                    string outputPath = GetOutputPath(cabinet, currentCode, processCounter);

                    Console.WriteLine($"Processing: {cabinet.CabinetFamilyName} | {cabinet.CabinetType} | Shelf {processCounter + 1}");
                    Console.WriteLine($"Reference: {referencePath}");
                    Console.WriteLine($"Output: {outputPath}");

                    bool success = Generate(cabinet, ipPlan, referencePath, outputPath, processCounter);
                    if (success)
                    {
                        Console.WriteLine("Generated successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Generation failed.");
                    }

                    processCounter++;
                } while (processCounter < processCount);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing cabinet: {ex.Message}");
                return false;
            }
        }

        private bool Generate(CabinetInfo cabinet, IpPlan ipPlan, string referencePathOrName, string outputPath, int processCounter)
        {
            try
            {
                List<string> lines;
                if (_useCloudTemplates)
                {
                    // Extract only the filename (e.g. "sh1MA5818.cfg") to query cloud storage
                    string fileName = Path.GetFileName(referencePathOrName);
                    lines = _templateService.DownloadTemplate(fileName);
                }
                else
                {
                    if (!File.Exists(referencePathOrName))
                    {
                        Console.WriteLine($"Reference file not found: {referencePathOrName}");
                        return false;
                    }
                    lines = new List<string>(File.ReadAllLines(referencePathOrName));
                }

                foreach (var modifier in _modifiers)
                {
                    lines = modifier.Modify(lines, cabinet, ipPlan, _context, processCounter);
                }

                string? directory = Path.GetDirectoryName(outputPath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllLines(outputPath, lines);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating file: {ex.Message}");
                return false;
            }
        }

        private int GetProcessCount(CabinetInfo cabinet)
        {
            if (cabinet.CabinetFamilyName == "Huawei" && cabinet.CabinetType == "MA5818")
                return 4;
            if (cabinet.CabinetFamilyName == "Nokia" && cabinet.CabinetType == "MODEL_B")
                return 2;
            return 1;
        }

        private string GetReferenceFilePath(CabinetInfo cabinet, int processCounter)
        {
            string path = "";
            switch (cabinet.CabinetType)
            {
                case "MA5818":
                    if (processCounter == 0 || processCounter == 2)
                        path = @"references\Models\MA5818\sh1MA5818.cfg"; // Using the actual files in directory
                    else
                        path = @"references\Models\MA5818\sh2MA5818.cfg"; // Using the actual files in directory
                    break;
                case "MA5600":
                    path = @"references\Models\MA_5600\MSAN-500-UPPER-2023.cfg";
                    break;
                case "GPON300":
                    path = @"references\Models\GPON_300\GPON-300.cfg";
                    break;
                case "GPON_T500":
                    path = @"references\Models\GPON_T500\GPON-T500.cfg";
                    break;
                case "MSAN500":
                    path = @"references\Models\MSAN_500\MSAN-500-UPPER-2023.cfg";
                    break;
                default:
                    path = @"references\Models\INDOOR 4 PORT.cfg"; // Default fallback
                    break;
            }
            return Path.GetFullPath(path);
        }

        private string GetOutputPath(CabinetInfo cabinet, string? currentCode, int processCounter)
        {
            string folderName;
            if (cabinet.Code2 != null)
            {
                string[] parts = cabinet.Code2.Split('-');
                folderName = cabinet.Code1 + " & " + parts[parts.Length - 1];
            }
            else
            {
                folderName = cabinet.Code1 ?? "Unknown";
            }

            string fileName;
            if (processCounter < 2)
            {
                fileName = $"{cabinet.Code1}-SH{(processCounter == 0 ? 1 : 2)}.cfg";
            }
            else
            {
                fileName = $"{cabinet.Code2}-SH{(processCounter == 2 ? 1 : 2)}.cfg";
            }

            string outputPath = Path.Combine("Outputs", folderName, fileName);
            return Path.GetFullPath(outputPath);
        }
    }
}
