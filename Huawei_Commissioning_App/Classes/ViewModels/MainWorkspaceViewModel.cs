using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Huawei_Commissioning_App.Classes.Services;
using Huawei_Commissioning_App.Classes.Strategies;

namespace Huawei_Commissioning_App.Classes.ViewModels
{
    public partial class MainWorkspaceViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _parent;

        public string UserKey { get; }
        public string Role { get; }
        public string Region { get; }

        public bool IsAdmin => Role == "Admin";
        public bool IsStaff => Role == "Admin" || Role == "Staff";
        public bool IsOutsource => true; // Outsource tab is always visible

        [ObservableProperty]
        private ViewModelBase? _generatorTab;

        [ObservableProperty]
        private ViewModelBase? _addCabinetTab;

        [ObservableProperty]
        private ViewModelBase? _manageTemplatesTab;

        public MainWorkspaceViewModel(MainWindowViewModel parent, string userKey, string role, string region)
        {
            _parent = parent;
            UserKey = userKey;
            Role = role;
            Region = region;

            // Instantiate tabs based on permissions
            GeneratorTab = new GeneratorViewModel(userKey, role, region);

            if (IsStaff)
            {
                AddCabinetTab = new AddCabinetViewModel(userKey);
            }

            if (IsAdmin)
            {
                ManageTemplatesTab = new ManageTemplatesViewModel();
            }
        }

        [RelayCommand]
        private void Logout()
        {
            _parent.NavigateToLogin();
        }
    }
}
