using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Debloat.xaml
    /// </summary>
    public partial class Debloat : Page, INavPage
    {
        public Debloat()
        {
            InitializeComponent();
        }

        public string PageTitle => "Debloat (under development)";
    }
}