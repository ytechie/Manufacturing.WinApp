using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Manufacturing.WinApp.Diagnostics;

namespace Manufacturing.WinApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            Loaded += OnLoaded;
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Frame.Navigate(typeof (Login));
        }
    }
}
