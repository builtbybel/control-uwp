using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page, INavPage
    {
        public Home()
        {
            InitializeComponent();

            labelWelcomeUsername.Content = Environment.UserName;
        }

        public string PageTitle => "Welcome";

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}