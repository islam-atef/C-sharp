using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Huawei_Commissioning_App.Classes.ViewModels;
using System;
using System.Linq;

namespace Huawei_Commissioning_App.Classes.Views
{
    public partial class AddCabinetView : UserControl
    {
        public AddCabinetView()
        {
            InitializeComponent();
        }

        private async void BrowseExcelClick(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel != null)
            {
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Select Excel IP Planning File",
                    AllowMultiple = false,
                    FileTypeFilter = new[]
                    {
                        new FilePickerFileType("Excel Sheets")
                        {
                            Patterns = new[] { "*.xlsx", "*.xls" }
                        }
                    }
                });

                if (files != null && files.Count > 0)
                {
                    var selectedPath = files[0].Path.LocalPath;
                    if (DataContext is AddCabinetViewModel vm)
                    {
                        vm.ExcelFilePath = selectedPath;
                    }
                }
            }
        }
    }
}
