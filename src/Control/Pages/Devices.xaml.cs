using System.Diagnostics;
using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Devices : Page, INavPage
    {
        public Devices()
        {
            InitializeComponent();
        }

        public string PageTitle => "Devices";

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}