using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Edge.xaml
    /// </summary>
    public partial class Edge : Page, INavPage
    {
        public Edge()
        {
            InitializeComponent();
        }

        public string PageTitle => "Edge (under development)";
    }
}