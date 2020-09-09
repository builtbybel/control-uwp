using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Navigation;

namespace Control
{
    public partial class MainWindow : Window
    {
        // App strings
        private readonly string _releaseURL = "https://raw.githubusercontent.com/builtbybel/control-uwp/master/latest.txt";

        private readonly string _releaseUpToDate = "There are currently no updates available.";
        private readonly string _releaseUnofficial = "You are using an unoffical version of ControlUWP.";

        internal static string GetCurrentVersionTostring() => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        public Version CurrentVersion = new Version(GetCurrentVersionTostring());
        public Version LatestVersion;

        private readonly string _appVersion = "ControlUWP" + "\nVersion " + GetCurrentVersionTostring();

        private readonly string _appInfo = "All infos about this project on\n" +
                        "github.com/builtbybel/control-uwp\n\n" +
                        "You can also follow me on\n" +
                        "twitter.com/builtbybel\r\n\n" +
                        "(C) 2020, Builtbybel (Website: www.builtbybel.com)";

        // General strings
        private string _fileName = "";

        private string _dialogFileTypes = "PS file (*.ps1)|*.ps1|Text file (*.txt)|*.txt";

        // PowerShell strings
        private readonly string _psError = "Settings folder not found.\nPlease check if it is stored in the installations directory of ControlUWP.";

        private readonly string _psSelection = "Select a page to retrieve the appropriate settings.";

        private void UpdateCheck()
        {
            try
            {
                WebRequest hreq = WebRequest.Create(_releaseURL);
                hreq.Timeout = 10000;
                hreq.Headers.Set("Cache-Control", "no-cache, no-store, must-revalidate");

                WebResponse hres = hreq.GetResponse();
                StreamReader sr = new StreamReader(hres.GetResponseStream());

                LatestVersion = new Version(sr.ReadToEnd().Trim());

                // Done and dispose!
                sr.Dispose();
                hres.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK);   // Update check failed!
            }

            var equals = LatestVersion.CompareTo(CurrentVersion);

            if (equals == 0)
            {
                _textDescription.Selection.Text = _appVersion + "\r\n\n" + _releaseUpToDate; // Up-to-date
                _textDescription.Focus();
            }
            else if (equals < 0)
            {
                _textDescription.Selection.Text = _appVersion + "\r\n\n" + _releaseUnofficial;  // Unofficial
                _textDescription.Focus();
            }
            else    // New release available!
            {
                _textDescription.Document.Blocks.Clear();
                _textDescription.Selection.Text = _appVersion + "\r\n\n" + _appInfo;
                _textDescription.Focus();

                if (MessageBox.Show("There is a new version available " + LatestVersion + "\n\nDo you want to open the @github/releases page?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Process.Start("https://github.com/builtbybel/control-uwp/releases/tag/" + LatestVersion);
                }
            }
        }

        private void _menuUpdateCheck_Click(object sender, RoutedEventArgs e)
        {
            _textDescription.Document.Blocks.Clear();
            UpdateCheck();
        }

        private void _menuGitHubLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/builtbybel/control-uwp");
        }

        private void _menuInfo_Click(object sender, RoutedEventArgs e)
        {
            _textDescription.Document.Blocks.Clear();
            _textDescription.Selection.Text = _appVersion + "\r\n\n" + _appInfo;
            _textDescription.Focus();
        }

        public MainWindow()
        {
            InitializeComponent();

            InitializeShell();
        }

        private void InitializeShell()
        {
            // Reset
            Reset();

            // Populate Categories
            PopulateCategory();

            // Welcome
            _labelWelcomeUsername.Content = Environment.UserName;
        }

        private void Reset()
        {
            // Clear description
            _textDescription.Document.Blocks.Clear();

            // Clear PS list
            _listPS.Items.Clear();

            // Clear PS console
            _textPS.Text = "";
        }

        /// <summary>
        /// Populate categories
        /// </summary>
        private void PopulateCategory()
        {
            // Clear list
            _listCategory.Items.Clear();

            string path = @"settings";

            if (Directory.Exists(path))
            {
                String[] dirs = System.IO.Directory.GetDirectories(@"settings");
                int i;
                for (i = 0; i < dirs.Length; i++)
                {
                    _listCategory.Items.Add(Path.GetFileNameWithoutExtension(dirs[i]));
                }
            }
            else MessageBox.Show(_psError);
        }

        /// <summary>
        /// Populate PS files
        /// </summary>
        private void PopulatePS()
        {
            // Reset
            Reset();

            DirectoryInfo dirs = new DirectoryInfo(@"settings\" + _listCategory.SelectedItem.ToString());
            FileInfo[] listFiles = dirs.GetFiles("*.ps1");
            foreach (FileInfo fi in listFiles)
            {
                _listPS.Items.Add(Path.GetFileNameWithoutExtension(fi.Name));
            }
        }

        private void _listCategory_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PopulatePS();
        }

        /// <summary>
        /// Read PS content line by line
        /// </summary>
        private void _listPS_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                string psdir = @"settings\" + _listCategory.SelectedItem.ToString() + "\\" + _listPS.SelectedItem.ToString() + ".ps1";

                using (StreamReader sr = new StreamReader(psdir, Encoding.Default))
                {
                    StringBuilder content = new StringBuilder();

                    // writes line by line to the StringBuilder until the end of the file is reached
                    while (!sr.EndOfStream)
                        content.AppendLine(sr.ReadLine());

                    // Show description
                    _textDescription.Selection.Text = string.Join(Environment.NewLine, File.ReadAllLines(psdir).Where(s => s.StartsWith("###")).Select(s => s.Substring(3).Replace("###", "\r\n")));

                    // Show code
                    _textPS.Text = content.ToString();
                }
            }
            catch { } // Off
        }

        /// <summary>
        /// Run PS files
        /// </summary>
        private string ApplySettings()
        {
            string applied = "Applied settings:\n";

            if (_listPS.SelectedItems.Count == 0)
            {
                MessageBox.Show(_psSelection, "", MessageBoxButton.OK);
                this.Effect = null;
            }

            foreach (var item in _listPS.SelectedItems)
            {
                // Blurring main window
                System.Windows.Media.Effects.BlurEffect myBlur = new System.Windows.Media.Effects.BlurEffect();
                myBlur.Radius = 5;
                this.Effect = myBlur;

                string psdir = @"settings\" + _listCategory.SelectedItem.ToString() + "\\" + item.ToString() + ".ps1";
                var ps1File = psdir;

                var equals = new[] { "Script", "All" };
                var str = _textDescription.Selection.Text;

                // Create ConsoleWindow
                if (equals.Any(str.Contains))
                {
                    var startInfo = new ProcessStartInfo()
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-executionpolicy bypass -file \"{ps1File}\"",
                        UseShellExecute = false,
                    };

                    Process.Start(startInfo).WaitForExit();
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

                    Process.Start(startInfo).WaitForExit();
                }

                applied += "\t" + item.ToString() + "\n";
            }

            return applied;
        }

        private void _buttonApply_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(ApplySettings());   // Apply settings
            this.Effect = null;                 // Remove blur effect
        }

        private void _buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog
            {
                Filter = _dialogFileTypes,
                DefaultExt = ".ps1",
                Multiselect = true,
                InitialDirectory = File.Exists(_fileName) ?
                  _fileName.Remove(_fileName.LastIndexOf('\\')) :
                  Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (openDlg.ShowDialog() == true)
            {
                foreach (string fileName in openDlg.FileNames)
                {
                    try
                    {
                        string strDestPath = @"settings\" + _listCategory.SelectedItem.ToString();
                        File.Copy(fileName, strDestPath + @"\" + Path.GetFileName(fileName));

                        // Refresh
                        PopulatePS();
                    }
                    catch (Exception ex)
                    { MessageBox.Show(ex.Message); }
                }
            }
        }

        private void SaveFile(bool saveAs = false)
        {
            if (File.Exists(_fileName) && !saveAs)
            {
                File.WriteAllText(_fileName, _textPS.Text);
                return;
            }

            SaveFileDialog saveDlg = ReturnSaveDialog();

            if (saveDlg.ShowDialog() == true)
            {
                File.WriteAllText(saveDlg.FileName, _textPS.Text);
                _fileName = saveDlg.FileName;
            }
        }

        private SaveFileDialog ReturnSaveDialog()
        {
            SaveFileDialog saveDlg = new SaveFileDialog
            {
                Filter = _dialogFileTypes,

                InitialDirectory = File.Exists(_fileName) ?
                    _fileName.Remove(_fileName.LastIndexOf('\\')) :
                    Directory.GetCurrentDirectory() + @"\settings",
                DefaultExt = "txt",
                AddExtension = true,
                FileName = _fileName.LastIndexOf('\\') != -1 ? _fileName.Substring(_fileName.LastIndexOf('\\') + 1) : _fileName
            };
            return saveDlg;
        }

        private void _listPS_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _buttonApply_Click(sender, e);
        }

        private void _buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void _buttonSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFile(true);
        }

        private void _buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "powershell_ise.exe";
                process.StartInfo.Arguments = "\"" + @"settings\" + _listCategory.SelectedItem.ToString() + "\\" + _listPS.SelectedItem.ToString() + ".ps1" + "\"";
                process.Start();
            }
            catch { }
        }

        private void _menuRefresh_Click(object sender, RoutedEventArgs e)
        {
            try { PopulatePS(); } catch { }
        }

        private void _menuSelectAll_Click(object sender, RoutedEventArgs e)
        {
            _listPS.SelectAll();
        }

        private void _menuUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            _listPS.SelectedIndex = -1;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}