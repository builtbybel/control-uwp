using System.Diagnostics;
using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Network&Internet.xaml
    /// </summary>
    public partial class NetworkAndInternet : Page, INavPage
    {
        public NetworkAndInternet()
        {
            InitializeComponent();
        }

        public string PageTitle => "Network & Internet";

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}