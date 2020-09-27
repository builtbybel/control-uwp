using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for PackageManagement.xaml
    /// </summary>
    public partial class PackageManagement : Page, INavPage
    {
        public PackageManagement()
        {
            InitializeComponent();
        }

        public string PageTitle => "Package Management";
    }
}