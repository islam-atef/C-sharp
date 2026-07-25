using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Huawei_Commissioning_App.Classes.ViewModels
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        [ObservableProperty]
        private string _userKey = "";

        [ObservableProperty]
        private string _errorMessage = "";

        [ObservableProperty]
        private bool _isBusy = false;

        public LoginViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(UserKey))
            {
                ErrorMessage = "Please enter an access key.";
                return;
            }

            ErrorMessage = "";
            IsBusy = true;

            // Run authentication in a background task to keep UI responsive
            var result = await Task.Run(() => AuthenticateUser(UserKey));

            IsBusy = false;

            if (result.Success)
            {
                _mainWindowViewModel.LoginSuccess(UserKey, result.AccessLevel, result.Region);
            }
            else
            {
                ErrorMessage = result.ErrorMsg;
            }
        }

        private (bool Success, string AccessLevel, string Region, string ErrorMsg) AuthenticateUser(string key)
        {
            if (AppConfig.IsFirebaseConfigured)
            {
                var validator = new CabinetValidator(AppConfig.DatabaseUrl, AppConfig.AuthSecret);
                var accessInfo = validator.ValidateKey(key);
                if (accessInfo != null)
                {
                    return (true, accessInfo.AccessLevel ?? "Outsource", accessInfo.Region ?? "All", "");
                }
                return (false, "", "", "Invalid access key. Please try again.");
            }
            else
            {
                // Offline fallback mode for development
                string upperKey = key.ToUpper();
                if (upperKey == "ADMIN")
                {
                    return (true, "Admin", "All", "");
                }
                else if (upperKey == "STAFF" || upperKey == "STUFF")
                {
                    return (true, "Staff", "11", "");
                }
                else if (upperKey == "OUTSOURCE")
                {
                    return (true, "Outsource", "12", "");
                }
                else
                {
                    // Default fallback
                    return (true, "Outsource", "All", "");
                }
            }
        }
    }
}
