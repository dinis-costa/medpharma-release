using Medpharma.Mobile.ItemViewModels;
using Medpharma.Mobile.Models;
using Medpharma.Mobile.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;

namespace Medpharma.Mobile.ViewModels
{
	public class AppointmentDetailsPageViewModel : ViewModelBase
	{
        private readonly INavigationService _navigationService;
        //private ObservableCollection<OrderItemViewModel> _orders;
        private AppointmentResponse _appointment;
        public AppointmentDetailsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
        }

        public AppointmentResponse Appointment
        {
            get => _appointment;
            set => SetProperty(ref _appointment, value);
        }

        //public ObservableCollection<OrderItemViewModel> Orders
        //{
        //    get => _orders;
        //    set => SetProperty(ref _orders, value);
        //}

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("appointment"))
            {
                Appointment = parameters.GetValue<AppointmentResponse>("appointment");
                Title = Appointment.date.Date.ToString("dd/mm/yyy");
            }
        }

    }
}
