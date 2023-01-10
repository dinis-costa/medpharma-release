using Medpharma.Mobile.Services;
using Medpharma.Mobile.ViewModels;
using Medpharma.Mobile.Views;
using Prism;
using Prism.Ioc;
using Syncfusion.Licensing;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace Medpharma.Mobile
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt/QHNqVVhmXFpFdEBBXHxAd1p/VWJYdVt5flBPcDwsT3RfQF9iS39Rd0ZgXX5ecXJVTg==;Mgo+DSMBPh8sVXJ0S0V+XE9CdlRDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3xSdERhWHxddHVVR2dfWQ==;ORg4AjUWIQA/Gnt2VVhjQlFacF9JXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRd0diW35dc3BURmFUVkw=;Nzc4MTYwQDMyMzAyZTMzMmUzME1KdU5ieDhxQ05uc0pNZS9NN3dTT1A2dFQ0VUhqSHIyajhYbHJFakhNWHM9;Nzc4MTYxQDMyMzAyZTMzMmUzMEtTRzdzcWFhQSsxZ1ptajZmQ2JJTWdGVEpXTy9mTllnc0Vnc01scGVhNE09;NRAiBiAaIQQuGjN/V0Z+X09EaFlCVmJLYVB3WmpQdldgdVRMZVVbQX9PIiBoS35RdERhWH1fc3dRR2BeVEZ+;Nzc4MTYzQDMyMzAyZTMzMmUzME9OdkNWTFVaaVI2OCt3eEpyYVAzWWFVZGVWdGJvNHhkQnoySTVMbko4UE09;Nzc4MTY0QDMyMzAyZTMzMmUzMEcxYTllUE5IZzYzLzNUTXBZaG5Nak85SU1NOGtDREtxdkt1MjhpZkpTc2M9;Mgo+DSMBMAY9C3t2VVhjQlFacF9JXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRd0diW35dc3BURmNeVEw=;Nzc4MTY2QDMyMzAyZTMzMmUzMEVhQnF2a3BZS1Bib2tkQThuVWdSbVo1Z205SmNFT0R5V0hCckMyY3o3bWM9;Nzc4MTY3QDMyMzAyZTMzMmUzMERYakZKajV6c3M1MTRta3E2M2VndXpLVnN3UzZwdVh3UCtxODhiRDlOU2c9;Nzc4MTY4QDMyMzAyZTMzMmUzME9OdkNWTFVaaVI2OCt3eEpyYVAzWWFVZGVWdGJvNHhkQnoySTVMbko4UE09");

            await NavigationService.NavigateAsync("NavigationPage/LoginPage");

            //await NavigationService.NavigateAsync
            //    ($"/{nameof(MainMenuPage)}/NavigationPage/{nameof(LoginPage)}");

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.Register<IApiService, ApiService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<AppointmentsPage, AppointmentsPageViewModel>();
            containerRegistry.RegisterForNavigation<AppointmentDetailsPage, AppointmentDetailsPageViewModel>();
            containerRegistry.RegisterForNavigation<OrdersPage, OrdersPageViewModel>();
            containerRegistry.RegisterForNavigation<OrdersDetailsPage, OrdersDetailsPageViewModel>();
            containerRegistry.RegisterForNavigation<MainMenuPage, MainMenuPageViewModel>();
        }
    }
}
