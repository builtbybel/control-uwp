using System.Diagnostics;
using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaktionslogik für EaseOfAccess.xaml
    /// </summary>
    public partial class EaseOfAccess : Page, INavPage
    {
        public EaseOfAccess()
        {
            InitializeComponent();
        }

        public string PageTitle => "Ease of Access";

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}