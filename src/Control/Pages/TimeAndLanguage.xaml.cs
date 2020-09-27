using System.Diagnostics;
using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Time&Language.xaml
    /// </summary>
    public partial class TimeAndLanguage : Page, INavPage
    {
        public TimeAndLanguage()
        {
            InitializeComponent();
        }

        public string PageTitle => "Time & Language";

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}