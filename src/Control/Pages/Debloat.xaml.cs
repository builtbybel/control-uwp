using ModernWpf.Controls;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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

            checkIsUserAdmin();
        }

        public string PageTitle => "Debloat";

        private AppxViewModel appxViewModel;

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void checkIsUserAdmin()
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                textReinstallApps.IsEnabled = false;
                textReinstallApps.Foreground = Brushes.Gray;
                textRemoveProvisionedApps.IsEnabled = false;
                textRemoveProvisionedApps.Foreground = Brushes.Gray;

                System.Windows.Controls.Control[] conts = { checkAllUsers };

                foreach (System.Windows.Controls.Control cont in conts)
                {
                    cont.IsEnabled = false;
                    cont.ToolTip = "Administrator privileges required";
                }
            }
        }

        private async void LoadApps()
        {
            appxViewModel = new AppxViewModel();
            appxViewModel.LoadAppx(checkAllUsers.IsChecked == true, checkExcludeStore.IsChecked == true);
            if (checkOnline.IsChecked == true)
            {
                await Task.Run(() => { appxViewModel.LoadAppxOnline(checkExcludeStore.IsChecked == true); });
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
                ProgressRing.IsActive = true;
                await Task.Run(() => { RemoveUWP.RemoveAppx(appxViewModel.apps); });
                LoadApps();
                ProgressRing.IsActive = false;
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

                ProgressRing.IsActive = true;
                await Task.Run(() => { proc.WaitForExit(); });
                ProgressRing.IsActive = false;
            }
        }

        private static string RunPsCommand(string command)
        {
            // Execute powershell script using command arguments as process
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "powershell.exe";
            startInfo.Arguments = command;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = false;

            // Start powershell process using process start info
            Process process = new Process { StartInfo = startInfo };
            process.Start();

            return process.StandardOutput.ReadToEnd();
        }

        private async void textReinstallApps_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog cd = new ContentDialog
            {
                Title = "Info",
                Content = "Do you want to reinstall all built-in Windows 10 apps?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            ContentDialogResult result = await cd.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ProgressRing.IsActive = true;
                await Task.Run(() => { RunPsCommand("Get-AppxPackage -AllUsers| Foreach {Add-AppxPackage -DisableDevelopmentMode -Register “$($_.InstallLocation)\\AppXManifest.xml”}"); });
                ProgressRing.IsActive = false;
            }
        }

        private async void textRemoveProvisionedApps_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog cd = new ContentDialog
            {
                Title = "Info",
                Content = "Provisoned apps are applications that Windows will attempt to reinstall during updates, or when a new user account is made. " +
                          "If you remove these apps you will have to install them manually through the Store app when making new accounts.\r\n\n" +
                          "Do you want to show all provisioned apps on this system?\nPress <CTRL> to select and remove mutliple apps at the same time.",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };

            ContentDialogResult result = await cd.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ProgressRing.IsActive = true;
                await Task.Run(() => { RunPsCommand("Get-AppxProvisionedPackage -online | Out-GridView -PassThru | Remove-AppxProvisionedPackage -online"); });
                ProgressRing.IsActive = false;
            }
        }
    }
}