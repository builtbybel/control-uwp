using ModernWpf.Controls;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        private void menuSelectAll_Click(object sender, RoutedEventArgs e)
        {
        }

        private void menuUnselectAll_Click(object sender, RoutedEventArgs e)
        {
        }

        private void RemoveFiles()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
            {
                if (checkWindowsUpdate.IsChecked == true)
                {
                    CleanStorage.WindowsUpdate();
                }

                if (checkDirectXShader.IsChecked == true)
                {
                    CleanStorage.DirectXShaderCache();
                }

                if (checkDeliveryOptimization.IsChecked == true)
                {
                    CleanStorage.DeliveryOptimization();
                }

                if (checkTemp.IsChecked == true)
                {
                    CleanStorage.Temp();
                }

                if (checkThumbnail.IsChecked == true)
                {
                    CleanStorage.ThumbnailCache();
                }

                if (checkMiniDumps.IsChecked == true)
                {
                    CleanStorage.MiniDumps();
                }

                if (checkClipboard.IsChecked == true)
                {
                    Clipboard.Clear();
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

                if (checkDNSCache.IsChecked == true)
                {
                    CleanStorage.DNSCache();
                }

                if (checkRecentDocuments.IsChecked == true)
                {
                    CleanStorage.RecentDocuments();
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

            ContentDialog cd = new ContentDialog
            {
                Title = "Storage Sense",
                Content = "Removing files has been successfully finished.",
                PrimaryButtonText = "OK",
            }; ContentDialogResult result = await cd.ShowAsync();

            buttonDoStorage.IsEnabled = true;
        }

        private void textDiskCleanup_Click(object sender, RoutedEventArgs e)
        {
            var cplPath = System.IO.Path.Combine(Environment.SystemDirectory, "cleanmgr.exe");
            Process.Start(cplPath);
        }
    }
}