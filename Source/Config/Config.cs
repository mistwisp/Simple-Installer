namespace Simple_Installer
{
    public class Config
    {
        public string zipFile = @".\Client.zip";                  //   .zip client file
        public string windowTitle = "Simple Installer";          //    Window Title
//------------------------------------------------------------------------------------------------------------------------
        public bool generateShortcut = true;                    //     Bool to enable desktop shortcut after extraction
        public string nameExecShortcut = "OrionAscension.exe"; //      Name of the executable file to create the shortcut
        public string shortcutName = "Orion Ascension";       //       Name of the desktop shortcut
        public string shortcutDesc = "Orion Ascension";      //        Description of the desktop shortcut
    }
}
