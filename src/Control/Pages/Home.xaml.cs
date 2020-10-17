using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page, INavPage
    {
        public Home()
        {
            InitializeComponent();

            labelWelcomeUsername.Content = Environment.UserName.ToUpper();
        }

        public string PageTitle => "Welcome";

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void textPrivacy_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/Privacy.xaml", UriKind.Relative));
        }

        private void textDebloat_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/Debloat.xaml", UriKind.Relative));
        }

        private void textStorageSense_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/System/StorageSense.xaml", UriKind.Relative));
        }

        private void textSystemInfo_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/System/SystemInfo.xaml", UriKind.Relative));
        }
    }
}