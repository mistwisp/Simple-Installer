using System.Windows;

namespace Simple_Installer
{
    public partial class Message : Window 
    {
        public Installer parentWindow {  get; set; }
        public Message(Installer parent) 
        {
            InitializeComponent();
            parentWindow = parent;
            TitleText.Text = parent.config.windowTitle;
            Show();
        }
        private void ClickMessage(object sender, RoutedEventArgs e) 
        {
            Close();
            parentWindow.Close();
        }
    }
}
