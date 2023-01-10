using Medpharma.Mobile.Models;
using Medpharma.Mobile.Views;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Medpharma.Mobile.ItemViewModels
{
    public class MenuItemViewModel : MainMenu
    {
            private readonly INavigationService _navigationService;
            private DelegateCommand _selectMenuCommand;

            public MenuItemViewModel(INavigationService navigationService)
            {
                _navigationService = navigationService;
            }

            public DelegateCommand SelectMenuCommand =>
                _selectMenuCommand ??
                (_selectMenuCommand = new DelegateCommand(SelectMenuAsync));

            private async void SelectMenuAsync()
            {
                await _navigationService.NavigateAsync(PageName);
            //await _navigationService.NavigateAsync
            //        ($"/{nameof(MainMenuPage)}/NavigationPage/{PageName}");
            }
    }
}
