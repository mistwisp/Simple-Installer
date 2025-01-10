using System;
using System.IO;
using System.Windows;
using IWshRuntimeLibrary;
using System.Windows.Forms;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Simple_Installer
{
    public partial class Installer : Window
    {
        public string folderPath = "";
        public Config config = new Config();
        public Message MessageWindow { get; set; }
        private CancellationTokenSource cts = new CancellationTokenSource();
        public Installer()
        {
            InitializeComponent();
            Show();
        }
        private void ClickClose(object sender, RoutedEventArgs e) 
        { 
            try 
            { 
                cts.Cancel(); 
                foreach (Window window in System.Windows.Application.Current.Windows) 
                { 
                    window.Close(); 
                } 
                System.Windows.Application.Current.Shutdown(); 
            } 
            catch (Exception ex) 
            { 
                Debug.WriteLine($"Shutdown error: {ex.Message}");
            } 
        }
        private void SelectFolder(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select a folder to install";
                dialog.ShowNewFolderButton = true;
                DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    folderPath = dialog.SelectedPath;
                    ExtractClient(folderPath);
                }
            }
        }
        private void CreateShortcut()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string targetFilePath = folderPath + "\\" + config.nameExecShortcut;
            string shortcutPath = Path.Combine(desktopPath, config.shortcutName + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetFilePath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetFilePath);
            shortcut.Description = config.shortcutDesc;
            shortcut.Save();
        }
        private void MessagePopup(Installer parent)
        {
            MessageWindow = new Message(parent)
            {
                Owner = this
            };
            MessageWindow.Show();
            MessageWindow.Left = Left;
            MessageWindow.Top = Top;
        }
        private void ExtractClient(string folderPath)
        {
            titlebar.Text = config.windowTitle;
            cli.Opacity = 0;
            cli.IsEnabled = false;
            unitFolder.Opacity = 1;
            unitText.Opacity = 1;
            InstallBarFiles.Opacity = 1;
            unitValue.Opacity = 1;
            unitFolder.Text = folderPath;
            _ = ExtractClientZip(folderPath);
        }
        public async Task ExtractClientZip(string outputFolder)
        {
            await ExtractZipWithProgressAsync(config.zipFile, outputFolder);
            if (config.generateShortcut)
                CreateShortcut();
            MessagePopup(this);
        }
        private async Task ExtractZipWithProgressAsync(string zipFilePath, string outputFolderPath)
        {


            await Task.Run(() =>
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    int totalEntries = archive.Entries.Count;
                    int processedEntries = 0;
                    foreach (var entry in archive.Entries)
                    {
                        string destinationPath = Path.Combine(outputFolderPath, entry.FullName);
                        if (string.IsNullOrWhiteSpace(entry.Name))
                        {
                            Directory.CreateDirectory(destinationPath);
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                            entry.ExtractToFile(destinationPath, overwrite: true);
                        }
                        processedEntries++;
                        Dispatcher.Invoke(() =>
                        {
                            int progressValue = (int)((double)processedEntries / totalEntries * 100);
                            InstallBarFiles.Value = progressValue;
                            unitValue.Text = progressValue.ToString() + "%";
                        });
                    }
                }
            });
        }
    }
}
