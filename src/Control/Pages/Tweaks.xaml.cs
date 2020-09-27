using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Tweaks.xaml
    /// </summary>
    public partial class Tweaks : Page, INavPage
    {
        public Tweaks()
        {
            InitializeComponent();
        }

        public string PageTitle => "Tweaks (under development)";
    }
}