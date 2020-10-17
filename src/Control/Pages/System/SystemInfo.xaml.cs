using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Control.Pages
{
    /// <summary>
    /// Interaction logic for SystemInfo.xaml
    /// </summary>
    public partial class SystemInfo : System.Windows.Controls.Page, INavPage
    {
        public SystemInfo()
        {
            InitializeComponent();

            GetSystemInfo();
        }

        public string PageTitle => "System Info";

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void textExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "My System information";     // Default filename
            dlg.DefaultExt = ".text";
            dlg.Filter = "Text file (.txt)|*.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                File.WriteAllText(dlg.FileName, textSystemInfo.Text);
            }
        }

        private void GetSystemInfo()
        {
            Process p = new Process();
            string lzBuffer = "";
            string sError = string.Empty;

            try
            {
                p.StartInfo.FileName = "systeminfo.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                using (StreamReader sr = p.StandardOutput)
                {
                    lzBuffer = sr.ReadToEnd();
                }
                using (StreamReader myError = p.StandardError)
                {
                    sError = myError.ReadToEnd();
                }

                // Set textbox with return output data
                textSystemInfo.Text = lzBuffer;

                // Some clean-up 
                lzBuffer = string.Empty;
            }
            catch (Exception err)
            {
                textSystemInfo.Text = err.Message;
            }
        }
    }
}