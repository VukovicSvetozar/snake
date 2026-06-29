using System.Windows;

namespace Snake.Views
{
    public partial class InfoDialog : Window
    {
        public InfoDialog(string icon, string message)
        {
            InitializeComponent();
            IconText.Text = icon;
            MessageText.Text = message;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}