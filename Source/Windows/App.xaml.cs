using System;
using System.Windows;
using System.Diagnostics;

namespace Simple_Installer
{
    public partial class App : Application
    {
        static bool IsRunAsAdministrator()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
        void AppMain(object sender, StartupEventArgs e)
        {
            if (!IsRunAsAdministrator())
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = Environment.GetCommandLineArgs()[0],
                    Verb = "runas"
                };
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                return;
            }
            else
            {
                _ = new Installer();
            }
        }
    }
}
