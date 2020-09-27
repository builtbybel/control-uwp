using System.Diagnostics;
using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Gaming.xaml
    /// </summary>
    public partial class Gaming : Page, INavPage
    {
        public Gaming()
        {
            InitializeComponent();
        }

        public string PageTitle => "Gaming";

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}