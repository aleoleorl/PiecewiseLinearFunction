using Avalonia.Controls;

namespace PLF_AvaloniaOriented.Views
{
    public partial class MessageBoxWindow : Window
    {
        public MessageBoxWindow(
            string textBoxText = "Ok or Cancel?",
            string headerText = "Warning",
            string okButtonText = "OK",
            string cancelButtonText = "Cancel")
        {
            InitializeComponent();

            this.Title = headerText;
            InfoArea.Text = textBoxText;
            OkButton.Content = okButtonText;
            CancelButton.Content = cancelButtonText;

            OkButton.Click += OkButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void OkButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(true);
        }

        private void CancelButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(false);
        }
    }
}
