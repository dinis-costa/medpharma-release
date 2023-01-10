using Medpharma.Mobile.ItemViewModels;
using Medpharma.Mobile.Models;
using Medpharma.Mobile.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Medpharma.Mobile.ViewModels
{
    public class MainMenuPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public MainMenuPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Main Menu";
            _navigationService = navigationService;
            LoadMenus();
        }

        public ObservableCollection<MenuItemViewModel> Menus { get; set; }

        public string User => Preferences.Get("user", "default_value");

        private void LoadMenus()
        {
            List<MainMenu> menus = new List<MainMenu>
            {
                new MainMenu
                {
                    Icon = "ic_card_giftcard",
                    PageName = $"{nameof(AppointmentsPage)}",
                    Title = "Appointments"
                },
                new MainMenu
                {
                    Icon = "ic_shopping_cart",
                    PageName = $"{nameof(OrdersPage)}",
                    Title = "Orders"
                }
            };

            Menus = new ObservableCollection<MenuItemViewModel>(
                menus.Select(m => new MenuItemViewModel(_navigationService)
                {
                    Icon = m.Icon,
                    PageName = m.PageName,
                    Title = m.Title,
                    IsLoginRequired = m.IsLoginRequired
                }).ToList());
        }
    }
}
