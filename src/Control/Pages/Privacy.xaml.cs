using ModernWpf.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for Privacy.xaml
    /// </summary>
    public partial class Privacy : System.Windows.Controls.Page, INavPage
    {
        public Privacy()
        {
            InitializeComponent();

            PopulatePS();
        }

        public string PageTitle => "Privacy";

        /// <summary>
        /// Populate PS files
        /// </summary>
        private void PopulatePS()
        {
            // Clear PS list
            listPS.Items.Clear();

            // Clear description
            textDescription.Text = "";

            string path = @"settings";

            if (Directory.Exists(path))
            {
                DirectoryInfo dirs = new DirectoryInfo(@"settings\privacy\");
                FileInfo[] listFiles = dirs.GetFiles("*.ps1");
                foreach (FileInfo fi in listFiles)
                {
                    listPS.Items.Add(Path.GetFileNameWithoutExtension(fi.Name));
                }
            }
            else MessageBox.Show("Settings folder not found.\nPlease check if it is stored in the installations directory of ControlUWP.");
        }

        /// <summary>
        /// Read PS content line by line
        /// </summary>
        private void listPS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string psdir = @"settings\privacy\" + "\\" + listPS.SelectedItem.ToString() + ".ps1";

                using (StreamReader sr = new StreamReader(psdir, Encoding.Default))
                {
                    StringBuilder content = new StringBuilder();

                    // Writes line by line to the StringBuilder until the end of the file is reached
                    while (!sr.EndOfStream)
                        content.AppendLine(sr.ReadLine());

                    // Show description
                    textDescription.Text = "";
                    textDescription.Text = string.Join(Environment.NewLine, File.ReadAllLines(psdir).Where(s => s.StartsWith("###")).Select(s => s.Substring(3).Replace("###", "\r\n")));
                }
            }
            catch { } // Off
        }

        /// <summary>
        /// Run PS files
        /// </summary>
        private async void ApplySettings()
        {
            string applied = "Applied settings:\n";

            // Add some cosmetics during runtime
            System.Windows.Media.Effects.BlurEffect myBlur = new System.Windows.Media.Effects.BlurEffect();             // Blurring page
            myBlur.Radius = 5;
            this.Effect = myBlur;
            listPS.IsEnabled = false;

            if (listPS.SelectedItems.Count == 0)
            {
                MessageBox.Show("No settings selected", "", MessageBoxButton.OK);
            }

            foreach (var item in listPS.SelectedItems)
            {
                string psdir = @"settings\privacy\" + "\\" + item.ToString() + ".ps1";
                var ps1File = psdir;

                var equals = new[] { "Script", "Group" };
                var str = textDescription.Text;

                // Create ConsoleWindow
                if (equals.Any(str.Contains))
                {
                    var startInfo = new ProcessStartInfo()
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-executionpolicy bypass -file \"{ps1File}\"",
                        UseShellExecute = false,
                    };

                    await Task.Run(() => { Process.Start(startInfo).WaitForExit(); });
                }
                else   // Silent
                {
                    var startInfo = new ProcessStartInfo()
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-executionpolicy bypass -file \"{ps1File}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };

                    await Task.Run(() => { Process.Start(startInfo).WaitForExit(); });
                }

                applied += "- " + item.ToString() + "\n";
            }

            textDescription.Text = applied;    // Show applied settings

            // Remove cosmetics
            this.Effect = null;
            listPS.IsEnabled = true;
        }

        private async void buttonApply_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog cd = new ContentDialog
            {
                Title = "Info",
                Content = "Are you sure you want to apply all selected settings?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No"
            };
            ContentDialogResult result = await cd.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                ApplySettings();
            }
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "powershell_ise.exe";
                process.StartInfo.Arguments = "\"" + @"settings\privacy\" + "\\" + listPS.SelectedItem.ToString() + ".ps1" + "\"";
                process.Start();
            }
            catch { }
        }

        private void menuRefresh_Click(object sender, RoutedEventArgs e)
        {
            try { PopulatePS(); } catch { }
        }

        private void menuSelectAll_Click(object sender, RoutedEventArgs e)
        {
            listPS.SelectAll();
        }

        private void menuUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            listPS.SelectedIndex = -1;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}