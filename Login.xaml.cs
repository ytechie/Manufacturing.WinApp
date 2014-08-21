using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Manufacturing.WinApp
{
    public partial class Login : Page
    {
        public Login()
        {
            Loaded += OnLoaded;
            this.InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Authenticate();
        }

        private async void Authenticate()
        {
            var authConfig = App.AuthenticationConfiguration;
            var authContext = new AuthenticationContext("https://login.windows.net/" + authConfig.DirectoryDomain);

            //Use this during development if you don't want your credentials cached
            //authContext.TokenCacheStore.Clear();

            var result = await authContext.AcquireTokenAsync(authConfig.AppRedirectUri, authConfig.AppClientId, new Uri(authConfig.ApiAppSignOnUrl));

            if (result.Status == AuthenticationStatus.Success)
            {
                App.BearerToken = result.AccessToken;
                App.CurrentUser = result.UserInfo;

                //Here is how to make a secure HTTP call with SecureHttpClient
                //var secureHttpClient = new SecureHttpClient(result.AccessToken);
                //var resp = await secureHttpClient.GetAsync("http://localhost:3184/api/echo?whoami=true");

                var rootFrame = (Frame)Window.Current.Content;
                rootFrame.Navigate(typeof(Views.Menu.MenuPage));
            }
            else
            {
                Authenticate();
            }
        }

    }
}
