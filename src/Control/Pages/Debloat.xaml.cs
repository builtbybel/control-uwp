using ModernWpf.Controls;
using System.Diagnostics;
using System.Windows;
using System.Threading.Tasks;

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
    }
}