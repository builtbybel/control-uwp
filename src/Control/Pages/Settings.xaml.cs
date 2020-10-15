using ModernWpf.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : System.Windows.Controls.Page, INavPage
    {
        private readonly string _releaseURL = "https://raw.githubusercontent.com/builtbybel/control-uwp/master/latest.txt";

        public string AssemblyVersion => Assembly.GetEntryAssembly().GetName().Version.ToString(3); // Embedd version to Xaml

        internal static string GetCurrentVersionTostring() => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        public Version CurrentVersion = new Version(GetCurrentVersionTostring());
        public Version LatestVersion;

        public Settings()
        {
            InitializeComponent();

            DataContext = this;  // Bind AssemblyVersion
        }

        public string PageTitle => "Settings";

        private async void UpdateCheck()
        {
            try
            {
                WebRequest hreq = WebRequest.Create(_releaseURL);
                hreq.Timeout = 10000;
                hreq.Headers.Set("Cache-Control", "no-cache, no-store, must-revalidate");

                WebResponse hres = hreq.GetResponse();
                StreamReader sr = new StreamReader(hres.GetResponseStream());

                LatestVersion = new Version(sr.ReadToEnd().Trim());

                // Done and dispose!
                sr.Dispose();
                hres.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK);   // Update check failed!
            }

            var equals = LatestVersion.CompareTo(CurrentVersion);

            if (equals == 0)
            {
                buttonCheckForUpdates.Content = "You are up-to-date";

            }
            else if (equals < 0)
            {
                buttonCheckForUpdates.Content = "You are up-to-date"; // Unofficial
            }
            else    // New release available!
            {
                var updateDialog = new ContentDialog()
                {
                    Title = "Update Available",
                    Content = $"There is an update available.\nWould you like to update now?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No"
                };
                var result = await updateDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    var ps = new ProcessStartInfo("https://github.com/builtbybel/control-uwp/releases/tag/" + LatestVersion)
                    {
                        UseShellExecute = true,
                    };
                    Process.Start(ps);
                }
            }
        }

        private void buttonCheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            UpdateCheck();
        }

        private void buttonOpenChangelog_Click(object sender, RoutedEventArgs e)
        {
            var ps = new ProcessStartInfo("https://github.com/builtbybel/control-uwp/releases/tag/" + CurrentVersion)
            {
                UseShellExecute = true,
            };
            Process.Start(ps);
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}