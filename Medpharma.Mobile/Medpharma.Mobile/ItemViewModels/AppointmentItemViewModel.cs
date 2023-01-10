using Medpharma.Mobile.Models;
using Medpharma.Mobile.Views;
using Prism.Commands;
using Prism.Navigation;
using System.Reflection;

namespace Medpharma.Mobile.ItemViewModels
{
    public class AppointmentItemViewModel : AppointmentResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectAppointmentCommand;

        public AppointmentItemViewModel(INavigationService navigationService)
        {
            
            _navigationService = navigationService;
        }

        public DelegateCommand SelectAppointmentCommand => _selectAppointmentCommand ?? (_selectAppointmentCommand = new DelegateCommand(SelectAppointmentAsync));

        private async void SelectAppointmentAsync()
        {
            NavigationParameters parameters = new NavigationParameters {
                {"appointment", this }
            };

            await _navigationService.NavigateAsync(nameof(AppointmentDetailsPage), parameters);
        }
    }
}
