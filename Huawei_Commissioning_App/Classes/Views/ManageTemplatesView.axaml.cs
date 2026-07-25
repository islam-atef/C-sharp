using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Huawei_Commissioning_App.Classes.ViewModels;
using System;
using System.Linq;

namespace Huawei_Commissioning_App.Classes.Views
{
    public partial class ManageTemplatesView : UserControl
    {
        public ManageTemplatesView()
        {
            InitializeComponent();
        }

        private async void BrowseAndUploadClick(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel != null)
            {
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Select Reference Template File (.cfg)",
                    AllowMultiple = false,
                    FileTypeFilter = new[]
                    {
                        new FilePickerFileType("Configuration Files")
                        {
                            Patterns = new[] { "*.cfg" }
                        }
                    }
                });

                if (files != null && files.Count > 0)
                {
                    var selectedPath = files[0].Path.LocalPath;
                    if (DataContext is ManageTemplatesViewModel vm)
                    {
                        await vm.UploadTemplateFileCommand.ExecuteAsync(selectedPath);
                    }
                }
            }
        }
    }
}
