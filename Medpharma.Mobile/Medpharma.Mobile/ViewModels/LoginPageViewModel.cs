using Medpharma.Mobile.Models;
using Medpharma.Mobile.Services;
using Medpharma.Mobile.Views;
using Prism.Commands;
using Prism.Navigation;
using System;
using Xamarin.Essentials;

namespace Medpharma.Mobile.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private string _password;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _loginCommand;
        private readonly IApiService _apiService;
        private readonly INavigationService _navigationService;

        public LoginPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            Title = "Login";
            IsEnabled = true;
            _apiService = apiService;
            _navigationService = navigationService;
        }

        public DelegateCommand LoginCommand => _loginCommand ?? (_loginCommand = new DelegateCommand(Login));

        public string Email { get; set; }

        public string Password { get => _password; set => SetProperty(ref _password, value); }

        public bool IsRunning { get => _isRunning; set => SetProperty(ref _isRunning, value); }

        public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter an email.", "Accept");
                Password = string.Empty;
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await App.Current.MainPage.DisplayAlert("Error", "You must enter a password.", "Accept"); Password = string.Empty;
                return;
            }

            var model = new BindingLogin();
            model.Password = this.Password;
            model.Email = this.Email;

            Response response = await _apiService.Login<MobileUser>("https://medpharma.azurewebsites.net//api/AccountMobile/Login", model);

            if (response.IsSuccess)
            {
                try
                {
                    await SecureStorage.SetAsync("user", $"{this.Email}");
                    Preferences.Set("user", $"{this.Email}");

                    await _navigationService.NavigateAsync(nameof(MainMenuPage));
                }
                catch (Exception ex)
                {
                    await App.Current.MainPage.DisplayAlert("Login Failed", "Please try again later.", "OK");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Login Failed!", "Confirm your credentials or try again later.", "OK");
            }
        }
    }
}