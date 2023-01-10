using Medpharma.Mobile.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Medpharma.Mobile.ViewModels
{
    public class OrdersDetailsPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private OrderResponse _order;

        public OrdersDetailsPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            
        }

        public OrderResponse Order
        {
            get => _order;
            set => SetProperty(ref _order, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("order"))
            {
                Order = parameters.GetValue<OrderResponse>("order");
                Title = $"Order Details - {Order.orderDate.Date.ToString("dd/MM/yyyy")}";
            }
        }
    }
}
