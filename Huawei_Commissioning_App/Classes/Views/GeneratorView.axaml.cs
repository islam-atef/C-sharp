using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Huawei_Commissioning_App.Classes.ViewModels;
using System;
using System.Linq;

namespace Huawei_Commissioning_App.Classes.Views
{
    public partial class GeneratorView : UserControl
    {
        public GeneratorView()
        {
            InitializeComponent();
        }

        private async void BrowseFolderClick(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel != null)
            {
                var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = "Select Commission Output Directory",
                    AllowMultiple = false
                });

                if (folders != null && folders.Count > 0)
                {
                    var selectedPath = folders[0].Path.LocalPath;
                    if (DataContext is GeneratorViewModel vm)
                    {
                        vm.FolderOutputPath = selectedPath;
                    }
                }
            }
        }
    }
}
