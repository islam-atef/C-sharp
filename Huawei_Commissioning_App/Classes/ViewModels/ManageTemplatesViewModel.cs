using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Huawei_Commissioning_App.Classes.Services;

namespace Huawei_Commissioning_App.Classes.ViewModels
{
    public partial class ManageTemplatesViewModel : ViewModelBase
    {
        private readonly FirebaseTemplateService _templateService;

        [ObservableProperty]
        private ObservableCollection<string> _templatesList = new ObservableCollection<string>();

        [ObservableProperty]
        private string? _selectedTemplateName;

        [ObservableProperty]
        private string _statusMessage = "";

        [ObservableProperty]
        private bool _isLoading = false;

        public ManageTemplatesViewModel()
        {
            _templateService = new FirebaseTemplateService(AppConfig.StorageBucket);
            
            // Auto load on init if configured
            if (AppConfig.IsFirebaseConfigured)
            {
                _ = RefreshTemplatesAsync();
            }
            else
            {
                StatusMessage = "Demo Mode: Firebase storage not configured. Simulated template list loaded.";
                TemplatesList.Add("sh1MA5818.cfg");
                TemplatesList.Add("sh2MA5818.cfg");
                TemplatesList.Add("GPON-300.cfg");
            }
        }

        [RelayCommand]
        private async Task RefreshTemplatesAsync()
        {
            if (!AppConfig.IsFirebaseConfigured) return;

            IsLoading = true;
            StatusMessage = "Loading templates from Firebase Storage...";

            await Task.Run(() =>
            {
                try
                {
                    var list = _templateService.ListTemplates();
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        TemplatesList.Clear();
                        foreach (var name in list)
                        {
                            TemplatesList.Add(name);
                        }
                        StatusMessage = $"Successfully loaded {TemplatesList.Count} templates.";
                    });
                }
                catch (Exception ex)
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        StatusMessage = $"Error listing templates: {ex.Message}";
                    });
                }
            });

            IsLoading = false;
        }

        [RelayCommand]
        private async Task UploadTemplateFileAsync(string localPath)
        {
            if (string.IsNullOrWhiteSpace(localPath) || !File.Exists(localPath))
            {
                StatusMessage = "Error: Invalid file selected.";
                return;
            }

            IsLoading = true;
            string fileName = Path.GetFileName(localPath);
            StatusMessage = $"Reading and uploading '{fileName}'...";

            await Task.Run(() =>
            {
                try
                {
                    string content = File.ReadAllText(localPath);
                    if (AppConfig.IsFirebaseConfigured)
                    {
                        bool success = _templateService.UploadTemplate(fileName, content);
                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            if (success)
                            {
                                StatusMessage = $"Success: Uploaded template '{fileName}'.";
                                _ = RefreshTemplatesAsync(); // Refresh list
                            }
                            else
                            {
                                StatusMessage = $"Failed to upload '{fileName}'.";
                            }
                        });
                    }
                    else
                    {
                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            StatusMessage = $"Demo Mode: Simulating upload of '{fileName}' (Content length: {content.Length} chars).";
                            if (!TemplatesList.Contains(fileName))
                            {
                                TemplatesList.Add(fileName);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        StatusMessage = $"Upload Error: {ex.Message}";
                    });
                }
            });

            IsLoading = false;
        }

        [RelayCommand]
        private async Task DeleteSelectedTemplateAsync()
        {
            if (string.IsNullOrEmpty(SelectedTemplateName))
            {
                StatusMessage = "Please select a template to delete.";
                return;
            }

            string nameToDelete = SelectedTemplateName;
            IsLoading = true;
            StatusMessage = $"Deleting '{nameToDelete}'...";

            await Task.Run(() =>
            {
                try
                {
                    if (AppConfig.IsFirebaseConfigured)
                    {
                        bool success = _templateService.DeleteTemplate(nameToDelete);
                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            if (success)
                            {
                                StatusMessage = $"Success: Deleted template '{nameToDelete}'.";
                                _ = RefreshTemplatesAsync(); // Refresh list
                            }
                            else
                            {
                                StatusMessage = $"Failed to delete '{nameToDelete}'.";
                            }
                        });
                    }
                    else
                    {
                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            TemplatesList.Remove(nameToDelete);
                            StatusMessage = $"Demo Mode: Deleted template '{nameToDelete}' from view list.";
                        });
                    }
                }
                catch (Exception ex)
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        StatusMessage = $"Delete Error: {ex.Message}";
                    });
                }
            });

            IsLoading = false;
        }
    }
}
