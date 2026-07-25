using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Huawei_Commissioning_App.Classes.Models;
using Huawei_Commissioning_App.Classes.Strategies;
using Huawei_Commissioning_App.Classes.Services;

namespace Huawei_Commissioning_App.Classes.ViewModels
{
    public partial class GeneratorViewModel : ViewModelBase
    {
        private readonly string _userKey;
        private readonly string _role;
        private readonly string _region;

        [ObservableProperty]
        private string _cabinetCode = "";

        [ObservableProperty]
        private string[] _cabinetFamilies = new[] { "Huawei", "Nokia" };

        [ObservableProperty]
        private string _selectedCabinetFamily = "Huawei";

        [ObservableProperty]
        private string[] _cabinetTypes = new[] { "MA5818", "MA5600", "GPON300", "GPON_T500", "MSAN500" };

        [ObservableProperty]
        private string _selectedCabinetType = "MA5818";

        [ObservableProperty]
        private string _folderOutputPath = "";

        [ObservableProperty]
        private string _consoleOutput = "";

        [ObservableProperty]
        private bool _isGenerating = false;

        public GeneratorViewModel(string userKey, string role, string region)
        {
            _userKey = userKey;
            _role = role;
            _region = region;
            
            // Set default output path
            FolderOutputPath = Path.Combine(Directory.GetCurrentDirectory(), "Outputs");

            // Redirect Console.Out to show logs in the UI text box
            var currentConsoleOut = Console.Out;
            var customWriter = new ObservableTextWriter(text =>
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    ConsoleOutput += text;
                });
            }, currentConsoleOut);
            Console.SetOut(customWriter);
        }

        partial void OnSelectedCabinetFamilyChanged(string value)
        {
            if (value == "Huawei")
            {
                CabinetTypes = new[] { "MA5818", "MA5600", "GPON300", "GPON_T500", "MSAN500" };
            }
            else if (value == "Nokia")
            {
                CabinetTypes = new[] { "MODEL_B" };
            }
            SelectedCabinetType = CabinetTypes.Length > 0 ? CabinetTypes[0] : "";
        }

        [RelayCommand]
        private async Task GenerateCommissionAsync()
        {
            if (string.IsNullOrWhiteSpace(CabinetCode))
            {
                Console.WriteLine("Error: Cabinet Code is required.");
                return;
            }

            ConsoleOutput = ""; // Clear console
            IsGenerating = true;

            await Task.Run(() =>
            {
                try
                {
                    Console.WriteLine($"Starting Commission Generation for Cabinet: {CabinetCode}...");

                    // Setup services
                    IIpProviderStrategy ipProvider;
                    CabinetValidator validator;
                    FirebaseLogService? logService = null;
                    FirebaseTemplateService templateService = new FirebaseTemplateService(AppConfig.StorageBucket);

                    if (AppConfig.IsFirebaseConfigured)
                    {
                        ipProvider = new FirebaseIpProviderStrategy(AppConfig.DatabaseUrl, AppConfig.AuthSecret);
                        validator = new CabinetValidator(AppConfig.DatabaseUrl, AppConfig.AuthSecret);
                        logService = new FirebaseLogService(AppConfig.DatabaseUrl, AppConfig.AuthSecret);
                    }
                    else
                    {
                        Console.WriteLine("Warning: Firebase is offline/unconfigured. Using local demo fallback.");
                        var inMemory = new InMemoryIpProviderStrategy();
                        inMemory.AddIpPlan(CabinetCode, new IpPlan
                        {
                            PopName = "Demo_POP_Name",
                            TedMgGatewayIp = "10.0.0.1",
                            TedMgSH1Ip = "10.0.0.2",
                            TedMgSH2Ip = "10.0.0.3",
                            MgGatewayIp = "20.0.0.1",
                            MgSH1Ip = "20.0.0.2",
                            MgSH2Ip = "20.0.0.3",
                            SigGatewayIp = "30.0.0.1",
                            SigSH1Ip = "30.0.0.2",
                            SigSH2Ip = "30.0.0.3",
                            FvnoEmGatewayIp = "40.0.0.1",
                            FvnoEmSH1Ip = "40.0.0.2",
                            FvnoEmSH2Ip = "40.0.0.3"
                        });
                        ipProvider = inMemory;
                        validator = new CabinetValidator("", "");
                    }

                    // Create CabinetInfo model
                    var cabinet = new CabinetInfo
                    {
                        CabinetFamilyName = SelectedCabinetFamily,
                        CabinetType = SelectedCabinetType,
                        Code1 = CabinetCode,
                        Code2 = SelectedCabinetFamily == "Huawei" && SelectedCabinetType == "MA5818" ? CabinetCode : null
                    };

                    string validationResult = validator.Validate(cabinet);
                    if (validationResult != "Accepted")
                    {
                        Console.WriteLine($"Cabinet Rejected: {validationResult}");
                        return;
                    }

                    // Setup context and engine
                    var context = new OperationalContext();
                    var engine = new CommissioningEngine(context, ipProvider, templateService, AppConfig.IsFirebaseConfigured);

                    // Execute processing
                    bool success = engine.ProcessCabinet(cabinet);

                    string runStatus = success ? "Success" : "Failed";
                    logService?.WriteLog(_userKey, CabinetCode, SelectedCabinetType, runStatus);

                    Console.WriteLine(success 
                        ? "\nCommission Generation Completed Successfully! Check output files." 
                        : "\nCommission Generation Failed. See log above.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Critical Error: {ex.Message}");
                }
            });

            IsGenerating = false;
        }
    }

    // Helper text writer to mirror Console logs to UI
    public class ObservableTextWriter : TextWriter
    {
        private readonly Action<string> _writeAction;
        private readonly TextWriter _fallbackWriter;

        public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;

        public ObservableTextWriter(Action<string> writeAction, TextWriter fallbackWriter)
        {
            _writeAction = writeAction;
            _fallbackWriter = fallbackWriter;
        }

        public override void Write(char value)
        {
            _writeAction(value.ToString());
            _fallbackWriter.Write(value);
        }

        public override void Write(string? value)
        {
            if (value != null)
            {
                _writeAction(value);
                _fallbackWriter.Write(value);
            }
        }

        public override void WriteLine(string? value)
        {
            if (value != null)
            {
                _writeAction(value + Environment.NewLine);
                _fallbackWriter.WriteLine(value);
            }
        }
    }
}
