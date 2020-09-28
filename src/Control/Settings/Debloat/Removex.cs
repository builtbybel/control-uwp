using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Control
{
    public static class RemoveUWP
    {
        /// <summary>
        /// Remove AppxPackages
        /// </summary>
        /// <param name="apps">Collection of packages to remove</param>
        /// <param name="allUsers">Remove for all users</param>
        public static void RemoveAppx(IEnumerable<Appx> apps)
        {
            foreach (Appx app in apps)
            {
                string c = GetRemovalCommand(app);
                if (!string.IsNullOrEmpty(c))
                {
                    RunPsCommand(c);
                }
            }
        }

        /// <summary>
        /// Determine what PowerShell command to use
        /// </summary>
        /// <param name="app"></param>
        /// <param name="allUsers"></param>
        /// <returns></returns>
        private static string GetRemovalCommand(Appx app) =>
            (app.Remove, app.OnlineProvisioned) switch
            {
                (false, _) => "",
                (true, true) => $"Remove-AppxProvisionedPackage {app.FullName} -Online",
                (true, false) => $"Remove-AppxPackage {app.FullName}"
            };

        /// <summary>
        /// Uses PowerShell to get list of Appx Packages
        /// </summary>
        /// <param name="allUsers"></param>
        /// <returns></returns>
        public static IEnumerable<Appx> LoadAppx(bool allUsers, bool noStore)
        {
            List<Appx> apps = new List<Appx>();
            StringBuilder argsBuilder = new StringBuilder();
            argsBuilder.Append("Get-AppxPackage");
            if (allUsers)
            {
                argsBuilder.Append(" -AllUsers");
            }
            argsBuilder.Append(" | Where-Object {$_.IsFramework -Match 'false' -and $_.NonRemovable -Match 'false'} | select-object -property @{N='Name';E={$_.Name}}, @{N='FullName';E={$_.PackageFullName}}, @{N='InstallLocation';E={$_.InstallLocation}}, @{N='OnlineProvisioned';E={$false}}");
            if (noStore)
            {
                argsBuilder.Append("| Where-Object {$_.Name -NotLike '*Microsoft.WindowsStore*' -and $_.Name -NotLike '*Microsoft.StorePurchaseApp*'}");
            }
            argsBuilder.Append(" | ConvertTo-Json");
            string output = RunPsCommand(argsBuilder.ToString());
            if (output.Length > 0)
            {
                apps = JsonSerializer.Deserialize<List<Appx>>(output);
            }
            return apps;
        }

        /// <summary>
        /// Uses PowerShell to get list of online Appx Packages
        /// </summary>
        /// <param name="allUsers"></param>
        /// <returns></returns>
        public static IEnumerable<Appx> LoadAppxOnline(bool noStore)
        {
            List<Appx> apps = new List<Appx>();
            StringBuilder argsBuilder = new StringBuilder();
            argsBuilder.Append("Get-AppxProvisionedPackage -Online | select-object -property @{N='Name';E={$_.DisplayName}}, @{N='FullName';E={$_.PackageName}}, @{N='installLocation';E={$_.InstallLocation}}, @{N='OnlineProvisioned';E={$true}} ");
            if (noStore)
            {
                argsBuilder.Append("| Where-Object {$_.Name -NotLike '*Microsoft.WindowsStore*' -and $_.Name -NotLike '*Microsoft.StorePurchaseApp*'}");
            }
            argsBuilder.Append("| ConvertTo-Json");
            string args = argsBuilder.ToString();
            string output = RunPsCommand(args);
            if (output.Length > 0)
            {
                apps = JsonSerializer.Deserialize<List<Appx>>(output);
            }
            return apps;
        }

        /// <summary>
        /// Runs a PowerShell command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private static string RunPsCommand(string command)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "powershell.exe";
            startInfo.Arguments = command;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            Process process = new Process { StartInfo = startInfo };
            process.Start();
            return process.StandardOutput.ReadToEnd();
        }
    }

    public class AppxViewModel
    {
        public ObservableCollection<Appx> apps { get; set; }

        private void InitApps()
        {
            apps ??= new ObservableCollection<Appx>();
        }

        public void LoadAppx(bool allUsers, bool noStore)
        {
            InitApps();
            foreach (Appx appx in RemoveUWP.LoadAppx(allUsers, noStore))
            {
                apps.Add(appx);
            }

            SortApps();
        }

        public void LoadAppxOnline(bool noStore)
        {
            InitApps();
            foreach (Appx appx in RemoveUWP.LoadAppxOnline(noStore))
            {
                apps.Add(appx);
            }

            SortApps();
        }

        public void SortApps()
        {
            apps = new ObservableCollection<Appx>(apps.OrderBy(x => x.Name));
        }
    }
}