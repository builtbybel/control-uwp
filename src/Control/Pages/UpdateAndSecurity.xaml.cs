using System.Diagnostics;
using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Update&Security.xaml
    /// </summary>
    public partial class UpdateAndSecurity : Page, INavPage
    {
        public UpdateAndSecurity()
        {
            InitializeComponent();
        }

        public string PageTitle => "Update & Security";

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}