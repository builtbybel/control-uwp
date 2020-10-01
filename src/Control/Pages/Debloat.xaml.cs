using ModernWpf.Controls;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Debloat.xaml
    /// </summary>
    public partial class Debloat : System.Windows.Controls.Page, INavPage
    {
        public Debloat()
        {
            InitializeComponent();

            LoadApps();
        }

        public string PageTitle => "Debloat";

        private AppxViewModel appxViewModel;

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void LoadApps()
        {
            appxViewModel = new AppxViewModel();
            appxViewModel.LoadAppx(checkAllUsers.IsChecked == true, checkExcludeStore.IsChecked == true);
            if (checkOnline.IsChecked == true)
            {
                appxViewModel.LoadAppxOnline(checkExcludeStore.IsChecked == true);
            }
            this.DataContext = appxViewModel;
            appxViewModel.SortApps();
            textInstalledApps.Text = $"{appxViewModel.apps.Count} apps found";
        }

        private void menuRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadApps();
        }

        private void menuSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (appxViewModel == null) return;
            foreach (Appx app in appxViewModel.apps)
            {
                app.Remove = true;
            }
        }

        private void menuUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            if (appxViewModel == null) return;
            foreach (Appx app in appxViewModel.apps)
            {
                app.Remove = !app.Remove;
            }
        }

        private async void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog cd = new ContentDialog
            {
                Title = "Info",
                Content = "Are you sure you want to remove all selected apps?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };
            ContentDialogResult result = await cd.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // Add some blur effect
                System.Windows.Media.Effects.BlurEffect myBlur = new System.Windows.Media.Effects.BlurEffect();
                myBlur.Radius = 5;
                this.Effect = myBlur;

                await Task.Run(() => { RemoveUWP.RemoveAppx(appxViewModel.apps); });
                LoadApps();

                this.Effect = null;
            }
        }

        private async void textRemoveOneDrive_Click(object sender, RoutedEventArgs e)
        {
  
            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            ContentDialog cd = new ContentDialog
            {
                Title = "Info",
                Content = "Do you really want to uninstall OneDrive?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            ContentDialogResult result = await cd.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                string sysEnv = Environment.GetEnvironmentVariable("SYSTEMROOT");
                proc.StartInfo.FileName = "taskkill"; proc.StartInfo.Arguments = "/F /IM OneDrive.exe";
                proc.Start();

                if (Environment.Is64BitOperatingSystem) { proc.StartInfo.FileName = (sysEnv + @"\SysWOW64\OneDriveSetup.exe"); }
                else proc.StartInfo.FileName = (sysEnv + @"\System32\OneDriveSetup.exe");
                proc.StartInfo.Arguments = "/uninstall";
                proc.Start();

                System.Windows.Media.Effects.BlurEffect myBlur = new System.Windows.Media.Effects.BlurEffect();
                myBlur.Radius = 5;
                this.Effect = myBlur;

                await Task.Run(() => { proc.WaitForExit(); });

                this.Effect = null;
            }
        }
    }
}