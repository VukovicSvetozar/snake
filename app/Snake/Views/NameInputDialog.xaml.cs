using System.Windows;
using System.Windows.Controls;

namespace Snake.Views
{
    public partial class NameInputDialog : Window
    {
        public string? PlayerName { get; private set; }

        public NameInputDialog()
        {
            InitializeComponent();
        }

        private void NameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TxtPlaceholder.Visibility = string.IsNullOrWhiteSpace(NameTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            if (!string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                NameMissingPopup.IsOpen = false;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                NameMissingPopup.IsOpen = true;
                return;
            }
            PlayerName = NameTextBox.Text;
            DialogResult = true;
            Close();
        }

    }
}