using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Huawei_Commissioning_App.Classes.ViewModels;

namespace Huawei_Commissioning_App
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Console.WriteLine("Creating MainWindow...");
                var window = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
                window.Topmost = true; // Force window to appear on top
                desktop.MainWindow = window;
                Console.WriteLine("MainWindow created and assigned.");
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
