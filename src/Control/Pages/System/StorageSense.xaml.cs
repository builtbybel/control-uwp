using ModernWpf.Controls;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for StorageSense.xaml
    /// </summary>
    public partial class StorageSense : System.Windows.Controls.Page, INavPage
    {
        public StorageSense()
        {
            InitializeComponent();
        }

        public string PageTitle => "Storage Sense";

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/System.xaml", UriKind.Relative));
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void RemoveFiles()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                if (checkTemp.IsChecked == true)
                {
                    CleanStorage.Temp();
                }
                if (checkMiniDumps.IsChecked == true)
                {
                    CleanStorage.MiniDumps();
                }
                if (checkPrefetch.IsChecked == true)
                {
                    CleanStorage.Prefetch();
                }
                if (checkLogs.IsChecked == true)
                {
                    CleanStorage.Logs();
                }
                if (checkErrorReports.IsChecked == true)
                {
                    CleanStorage.ErrorReports();
                }
                if (checkRecycleBin.IsChecked == true)
                {
                    CleanStorage.RecycleBin();
                }
            }));
        }

        private async void buttonDoStorage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            buttonDoStorage.IsEnabled = false;

            await Task.Run(() => { RemoveFiles(); });

            buttonDoStorage.IsEnabled = true;

            ContentDialog cd = new ContentDialog
            {
                Title = "Storage Sense",
                Content = "Removing files has been successfully finished.",
                PrimaryButtonText = "OK",
            }; ContentDialogResult result = await cd.ShowAsync();
        }
    }
}