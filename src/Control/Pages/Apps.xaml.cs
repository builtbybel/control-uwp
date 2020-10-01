using System.Diagnostics;
using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Apps.xaml
    /// </summary>
    public partial class Apps : Page, INavPage
    {
        public Apps()
        {
            InitializeComponent();
        }

        public string PageTitle => "Apps";

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}