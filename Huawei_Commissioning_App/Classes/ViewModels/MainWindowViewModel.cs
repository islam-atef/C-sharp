using CommunityToolkit.Mvvm.ComponentModel;

namespace Huawei_Commissioning_App.Classes.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase? _currentView;

        public MainWindowViewModel()
        {
            NavigateToLogin();
        }

        public void NavigateToLogin()
        {
            CurrentView = new LoginViewModel(this);
        }

        public void LoginSuccess(string userKey, string accessLevel, string region)
        {
            CurrentView = new MainWorkspaceViewModel(this, userKey, accessLevel, region);
        }
    }
}
