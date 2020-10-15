using ModernWpf.Controls;
using ModernWpf.Media.Animation;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly IReadOnlyDictionary<string, Type> Pages = new Dictionary<string, Type>
        {
            { "Home", typeof(Pages.Home) },
            { "Apps", typeof(Pages.Apps) },
            { "Debloat", typeof(Pages.Debloat) },
            { "Devices", typeof(Pages.Devices) },
            { "Ease of Access", typeof(Pages.EaseOfAccess) },
            { "Edge", typeof(Pages.Edge) },
            { "Gaming", typeof(Pages.Gaming) },
            { "Network & Internet", typeof(Pages.NetworkAndInternet) },
            { "Package Management", typeof(Pages.PackageManagement) },
            { "Personalization", typeof(Pages.Personalization) },
            { "Privacy", typeof(Pages.Privacy) },
            { "Search", typeof(Pages.Search) },
            { "System", typeof(Pages.SystemPage) },
            { "Time & Language", typeof(Pages.TimeAndLanguage) },
            { "Update & Security", typeof(Pages.UpdateAndSecurity) }
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                frame.Navigate(typeof(Pages.Settings));
            }
            else
            {
                var selectedItem = (NavigationViewItem)args.SelectedItem;
                if (selectedItem != null)
                {
                    var pageType = Pages[(string)selectedItem.Tag];
                    // https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.animation.drillinnavigationtransitioninfo?view=winrt-19041
                    frame.Navigate(pageType, null, new SuppressNavigationTransitionInfo());
                }
            }
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            BackRequested();
        }

        private bool BackRequested()
        {
            if (!frame.CanGoBack) return false;

            if (navigationView.IsPaneOpen &&
                (navigationView.DisplayMode == NavigationViewDisplayMode.Minimal
                 || navigationView.DisplayMode == NavigationViewDisplayMode.Compact))
            {
                return false;
            }
            frame.GoBack();
            return true;
        }
    }
}