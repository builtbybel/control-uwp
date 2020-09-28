using System.Diagnostics;
using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for PackageManagement.xaml
    /// </summary>
    public partial class PackageManagement : Page, INavPage
    {
        public PackageManagement()
        {
            InitializeComponent();
        }

        public string PageTitle => "Package Management (under development)";

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}