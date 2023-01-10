using Example;
using Medpharma.Mobile.ItemViewModels;
using Medpharma.Mobile.Models;
using Medpharma.Mobile.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Medpharma.Mobile.ViewModels
{
    public class OrdersPageViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly INavigationService _navigationService;
        private ObservableCollection<OrderItemViewModel> _orders;
        private bool _isRunning;
        private string _search;
        private List<OrderResponse> _myOrders;
        private DelegateCommand _searchCommand;

        public OrdersPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            Title = "Orders";
            _apiService = apiService;
            _navigationService = navigationService;
            LoadOrdersAsync();
        }

        public DelegateCommand SearchCommand => _searchCommand ?? (_searchCommand = new DelegateCommand(LoadOrdersAsync));

        public string Search
        {
            get => _search;
            set
            {
                SetProperty(ref _search, value);

                ShowOrders();
            }
        }
        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public ObservableCollection<OrderItemViewModel> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        private async void LoadOrdersAsync()
        {
            IsRunning = true;

            Response response = await _apiService.GetListAsync<OrderResponse>("https://medpharma.azurewebsites.net/api/OrdersMobile/getUserOrders");

            IsRunning = false;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", response.Message, "Accept");
                return;
            }

            _myOrders = (List<OrderResponse>)response.Result;
            ShowOrders();
        }

        private void ShowOrders()
        {
            if (string.IsNullOrEmpty(Search))
            {
                Orders = new ObservableCollection<OrderItemViewModel>(_myOrders.Select(p =>
                new OrderItemViewModel(_navigationService)
                {
                    id = p.id,
                    orderDate = p.orderDate,
                    deliveryDate = p.deliveryDate,
                    items = p.items,
                    customer = p.customer,
                    lines = p.lines,
                    quantity = p.quantity,
                    cValue = p.cValue,
                    orderDateLocal = p.orderDateLocal,
                    orderSent = p.orderSent,
                }).ToList());
            }
            else
            {
                Orders = new ObservableCollection<OrderItemViewModel>(_myOrders.Select(p => new OrderItemViewModel(_navigationService)
                {
                    id = p.id,
                    orderDate = p.orderDate,
                    deliveryDate = p.deliveryDate,
                    items = p.items,
                    customer = p.customer,
                    lines = p.lines,
                    quantity = p.quantity,
                    cValue = p.cValue,
                    orderDateLocal = p.orderDateLocal,
                    orderSent = p.orderSent,
                    
                })./*Where(p => p.deliveryDate.ToLower().Contains(Search.ToLower())).*/ToList());
            }
        }
    }
}
