using System.Diagnostics;
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
    }
}