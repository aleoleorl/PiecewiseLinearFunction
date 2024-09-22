using Avalonia.Controls;

namespace PLF_AvaloniaOriented.Views
{
    public partial class RequestBoxWindow : Window
    {
        public string TextBoxText { get; private set; }

        public RequestBoxWindow(
            string textBoxText = "Ok or Cancel?",
            string headerText = "Warning",
            string defaultText="",
            string okButtonText = "OK",
            string cancelButtonText = "Cancel")
        {
            InitializeComponent();

            this.Title = headerText;
            InfoArea.Content = textBoxText;
            TextArea.Text = defaultText;
            OkButton.Content = okButtonText;
            CancelButton.Content = cancelButtonText;

            OkButton.Click += OkButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void OkButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            TextBoxText = TextArea.Text;
            Close(true);
        }

        private void CancelButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Close(false);
        }
    }
}