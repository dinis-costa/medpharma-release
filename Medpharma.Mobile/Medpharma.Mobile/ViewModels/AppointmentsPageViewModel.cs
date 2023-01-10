using Medpharma.Mobile.Models;
using Medpharma.Mobile;
using Medpharma.Mobile.Services;
using Medpharma.Mobile.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Medpharma.Mobile.ItemViewModels;

namespace Medpharma.Mobile.ViewModels
{
    public class AppointmentsPageViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly INavigationService _navigationService;
        private ObservableCollection<AppointmentItemViewModel> _appointments;
        private bool _isRunning;
        private string _search;
        private List<AppointmentResponse> _myAppointments;
        private DelegateCommand _searchCommand;

        public AppointmentsPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            Title = "Appointments";
            _apiService = apiService;
            _navigationService = navigationService;
            LoadAppointmentsAsync();
        }

        public DelegateCommand SearchCommand => _searchCommand ?? (_searchCommand = new DelegateCommand(LoadAppointmentsAsync));

        public string Search
        {
            get => _search;
            set
            {
                SetProperty(ref _search, value);

                ShowAppointments();
            }
        }
        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public ObservableCollection<AppointmentItemViewModel> Appointments
        {
            get => _appointments;
            set => SetProperty(ref _appointments, value);
        }

        private async void LoadAppointmentsAsync()
        {
            IsRunning = true;

            Response response = await _apiService.GetListAsync<AppointmentResponse>("https://medpharma.azurewebsites.net/api/AppointmentsMobile/getUserAppointments");

            IsRunning = false;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                return;
            }

            _myAppointments = (List<AppointmentResponse>)response.Result;
            ShowAppointments();
        }

        private void ShowAppointments()
        {
            if (string.IsNullOrEmpty(Search))
            {
                Appointments = new ObservableCollection<AppointmentItemViewModel>(_myAppointments.Select(p =>
                new AppointmentItemViewModel(_navigationService)
                {
                    appId = p.appId,
                    doctor = p.doctor,
                    speciality = p.speciality,
                    date = p.date.Date,
                    timeSlot = p.timeSlot,
                    status = p.status
                }).ToList());
            }
            else
            {
                Appointments = new ObservableCollection<AppointmentItemViewModel>(_myAppointments.Select(p => new AppointmentItemViewModel(_navigationService)
                {
                    appId = p.appId,
                    doctor = p.doctor,
                    speciality = p.speciality,
                    date = p.date,
                    timeSlot = p.timeSlot,
                    status = p.status
                }).Where(p => p.speciality.ToLower().Contains(Search.ToLower())).ToList());
            }
        }
    }
}