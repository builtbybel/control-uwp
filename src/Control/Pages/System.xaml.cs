using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for System.xaml
    /// </summary>
    public partial class SystemPage : Page, INavPage
    {
        public SystemPage()
        {
            InitializeComponent();
        }

        public string PageTitle => "System " + "(" + System.Environment.MachineName + ")";

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void textCplComputername_Click(object sender, RoutedEventArgs e)
        {
            var cplPath = System.IO.Path.Combine(Environment.SystemDirectory, "SystemPropertiesComputerName");
            Process.Start(new ProcessStartInfo(cplPath) { UseShellExecute = true });
        }

        private void textStorageSense_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/System/StorageSense.xaml", UriKind.Relative));
        }
    }
}