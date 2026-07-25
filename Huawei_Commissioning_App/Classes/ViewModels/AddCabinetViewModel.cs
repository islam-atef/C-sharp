using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Huawei_Commissioning_App.Classes.Models;
using Huawei_Commissioning_App.Classes.Strategies;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace Huawei_Commissioning_App.Classes.ViewModels
{
    public partial class AddCabinetViewModel : ViewModelBase
    {
        private readonly string _userKey;

        [ObservableProperty] private string _cabinetCode = "";
        [ObservableProperty] private string _popName = "";
        [ObservableProperty] private string _tedMgGatewayIp = "";
        [ObservableProperty] private string _tedMgSH1Ip = "";
        [ObservableProperty] private string _tedMgSH2Ip = "";
        [ObservableProperty] private string _mgGatewayIp = "";
        [ObservableProperty] private string _mgSH1Ip = "";
        [ObservableProperty] private string _mgSH2Ip = "";
        [ObservableProperty] private string _mgSH3Ip = "";
        [ObservableProperty] private string _sigGatewayIp = "";
        [ObservableProperty] private string _sigSH1Ip = "";
        [ObservableProperty] private string _sigSH2Ip = "";
        [ObservableProperty] private string _fvnoEmGatewayIp = "";
        [ObservableProperty] private string _fvnoEmSH1Ip = "";
        [ObservableProperty] private string _fvnoEmSH2Ip = "";

        [ObservableProperty] private string _excelFilePath = "";
        [ObservableProperty] private string _statusMessage = "";
        [ObservableProperty] private bool _isUploading = false;

        public AddCabinetViewModel(string userKey)
        {
            _userKey = userKey;
        }

        [RelayCommand]
        private async Task AddPlanManualAsync()
        {
            if (string.IsNullOrWhiteSpace(CabinetCode))
            {
                StatusMessage = "Error: Cabinet Code is required.";
                return;
            }

            StatusMessage = "Uploading IP plan to cloud...";
            IsUploading = true;

            await Task.Run(() =>
            {
                try
                {
                    var plan = new IpPlan
                    {
                        PopName = PopName,
                        TedMgGatewayIp = TedMgGatewayIp,
                        TedMgSH1Ip = TedMgSH1Ip,
                        TedMgSH2Ip = TedMgSH2Ip,
                        MgGatewayIp = MgGatewayIp,
                        MgSH1Ip = MgSH1Ip,
                        MgSH2Ip = MgSH2Ip,
                        MgSH3Ip = MgSH3Ip,
                        SigGatewayIp = SigGatewayIp,
                        SigSH1Ip = SigSH1Ip,
                        SigSH2Ip = SigSH2Ip,
                        FvnoEmGatewayIp = FvnoEmGatewayIp,
                        FvnoEmSH1Ip = FvnoEmSH1Ip,
                        FvnoEmSH2Ip = FvnoEmSH2Ip
                    };

                    if (AppConfig.IsFirebaseConfigured)
                    {
                        var provider = new FirebaseIpProviderStrategy(AppConfig.DatabaseUrl, AppConfig.AuthSecret);
                        bool success = provider.AddIpPlan(CabinetCode, plan);
                        
                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            if (success)
                            {
                                StatusMessage = $"Success: IP Plan for Cabinet {CabinetCode} uploaded to Firebase.";
                                ClearForm();
                            }
                            else
                            {
                                StatusMessage = "Failed: Database returned write error.";
                            }
                        });
                    }
                    else
                    {
                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            StatusMessage = "Demo Mode: Validated local write (Firebase is not configured).";
                        });
                    }
                }
                catch (Exception ex)
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        StatusMessage = $"Error: {ex.Message}";
                    });
                }
            });

            IsUploading = false;
        }

        [RelayCommand]
        private async Task ImportExcelAsync()
        {
            if (string.IsNullOrWhiteSpace(ExcelFilePath) || !File.Exists(ExcelFilePath))
            {
                StatusMessage = "Error: Please select a valid Excel file.";
                return;
            }

            StatusMessage = "Importing plans from Excel file...";
            IsUploading = true;

            await Task.Run(() =>
            {
                try
                {
                    int importedCount = 0;
                    using (FileStream file = new FileStream(ExcelFilePath, FileMode.Open, FileAccess.Read))
                    {
                        XSSFWorkbook workbook = new XSSFWorkbook(file);
                        ISheet sheet = workbook.GetSheetAt(0); // Read first sheet

                        // Determine strategy or read rows manually
                        for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++)
                        {
                            IRow row = sheet.GetRow(rowIdx);
                            if (row == null) continue;

                            string cabCode = row.GetCell(0)?.ToString() ?? "";
                            if (string.IsNullOrWhiteSpace(cabCode)) continue;

                            // Populate IpPlan columns
                            var plan = new IpPlan
                            {
                                PopName = row.GetCell(1)?.ToString() ?? "",
                                TedMgGatewayIp = row.GetCell(2)?.ToString() ?? "",
                                TedMgSH1Ip = row.GetCell(3)?.ToString() ?? "",
                                TedMgSH2Ip = row.GetCell(4)?.ToString() ?? "",
                                MgGatewayIp = row.GetCell(5)?.ToString() ?? "",
                                MgSH1Ip = row.GetCell(6)?.ToString() ?? "",
                                MgSH2Ip = row.GetCell(7)?.ToString() ?? "",
                                MgSH3Ip = row.GetCell(8)?.ToString() ?? "",
                                SigGatewayIp = row.GetCell(9)?.ToString() ?? "",
                                SigSH1Ip = row.GetCell(10)?.ToString() ?? "",
                                SigSH2Ip = row.GetCell(11)?.ToString() ?? "",
                                FvnoEmGatewayIp = row.GetCell(12)?.ToString() ?? "",
                                FvnoEmSH1Ip = row.GetCell(13)?.ToString() ?? "",
                                FvnoEmSH2Ip = row.GetCell(14)?.ToString() ?? ""
                            };

                            if (AppConfig.IsFirebaseConfigured)
                            {
                                var provider = new FirebaseIpProviderStrategy(AppConfig.DatabaseUrl, AppConfig.AuthSecret);
                                provider.AddIpPlan(cabCode, plan);
                            }
                            importedCount++;
                        }
                    }

                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        StatusMessage = $"Success: Imported {importedCount} IP plans from Excel to the cloud.";
                        ExcelFilePath = "";
                    });
                }
                catch (Exception ex)
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        StatusMessage = $"Import Error: {ex.Message}";
                    });
                }
            });

            IsUploading = false;
        }

        private void ClearForm()
        {
            CabinetCode = "";
            PopName = "";
            TedMgGatewayIp = "";
            TedMgSH1Ip = "";
            TedMgSH2Ip = "";
            MgGatewayIp = "";
            MgSH1Ip = "";
            MgSH2Ip = "";
            MgSH3Ip = "";
            SigGatewayIp = "";
            SigSH1Ip = "";
            SigSH2Ip = "";
            FvnoEmGatewayIp = "";
            FvnoEmSH1Ip = "";
            FvnoEmSH2Ip = "";
        }
    }
}
