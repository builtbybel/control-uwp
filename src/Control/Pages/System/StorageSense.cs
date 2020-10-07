using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Control
{
    public static class FlushDns
    {
        [DllImport("dnsapi", EntryPoint = "DnsFlushResolverCache")]
        public static extern void FlushCache();
    }

    internal static class CleanStorage
    {
        private enum RecycleFlag : int

        {
            SHERB_NOCONFIRMATION = 0x00000001, // No confirmation, when emptying

            SHERB_NOPROGRESSUI = 0x00000001, // No progress tracking window during the emptying of the recycle bin

            SHERB_NOSOUND = 0x00000004 // No sound when the emptying of the recycle bin is complete
        }

        [DllImport("Shell32.dll")]
        private static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, RecycleFlag dwFlags);

        internal static readonly string System32Folder = Environment.GetFolderPath(Environment.SpecialFolder.System);
        internal static readonly string TempFolder = Path.GetTempPath();
        internal static readonly string ProfileAppDataRoaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        internal static readonly string ProgramData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        internal static readonly string ProfileAppDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        internal static readonly string ProfileAppDataLocalLow = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low";
        internal static readonly string OSDrive = System32Folder.Substring(0, 3);
        internal static readonly string OSDriveWindows = Environment.GetEnvironmentVariable("WINDIR", EnvironmentVariableTarget.Machine);

        internal static void EmptyFolder(string path)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);

                foreach (FileInfo file in di.GetFiles())
                {
                    try
                    {
                        file.IsReadOnly = false;
                        file.Delete();
                    }
                    catch { }
                }

                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    try
                    {
                        dir.Delete(true);
                    }
                    catch { }
                }
            }
            catch { }
        }

        internal static void WindowsUpdate()
        {
            EmptyFolder(OSDriveWindows + "\\SoftwareDistribution\\Download");
            EmptyFolder(OSDriveWindows + "\\Installer\\$PatchCache$");
        }

        internal static void DirectXShaderCache()
        {
            EmptyFolder(ProfileAppDataLocal + "\\D3DSCache");
        }

        internal static void DeliveryOptimization()
        {
            EmptyFolder(OSDriveWindows + "\\ServiceProfiles\\NetworkService\\AppData\\Local\\Microsoft\\Windows\\DeliveryOptimization");
        }

        internal static void Temp()
        {
            EmptyFolder(TempFolder);
            EmptyFolder(OSDriveWindows + "\\Temp");
        }

        internal static void ThumbnailCache()
        {
            EmptyFolder(ProfileAppDataLocal + "\\Microsoft\\Windows\\Explorer");
        }

        internal static void Logs()
        {
            EmptyFolder(System32Folder + "\\LogFiles"); // Windows logs
            EmptyFolder(OSDrive + "\\inetpub\\logs\\LogFiles"); // IIS logs
        }

        internal static void MiniDumps()
        {
            EmptyFolder(OSDriveWindows + "\\Minidump");
        }

        internal static void Prefetch()
        {
            EmptyFolder(OSDriveWindows + "\\Prefetch");
        }

        internal static void ErrorReports()
        {
            EmptyFolder(ProfileAppDataLocal + "\\Microsoft\\Windows\\WER\\ReportArchive");
            EmptyFolder(ProfileAppDataLocal + "\\Microsoft\\Windows\\WER\\ReportQueue");
            EmptyFolder(ProfileAppDataLocal + "\\Microsoft\\Windows\\WER\\Temp");
            EmptyFolder(ProfileAppDataLocal + "\\Microsoft\\Windows\\WER\\ERC");
            EmptyFolder(ProgramData + "\\Microsoft\\Windows\\WER\\ReportArchive");
            EmptyFolder(ProgramData + "\\Microsoft\\Windows\\WER\\ReportQueue");
            EmptyFolder(ProgramData + "\\Microsoft\\Windows\\WER\\Temp");
            EmptyFolder(ProgramData + "\\Microsoft\\Windows\\WER\\ERC");
        }

        internal static void DNSCache()
        {
            FlushDns.FlushCache();
        }

        internal static void RecentDocuments()
        {
            EmptyFolder(ProfileAppDataRoaming + "\\Microsoft\\Windows\\Recent");
        }

        internal static void RecycleBin()
        {
            SHEmptyRecycleBin(IntPtr.Zero, null, RecycleFlag.SHERB_NOSOUND | RecycleFlag.SHERB_NOCONFIRMATION);
        }
    }
}