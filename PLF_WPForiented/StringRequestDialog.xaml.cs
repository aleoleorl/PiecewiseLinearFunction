using System.Windows;

namespace PLF_WPForiented
{
    public partial class StringRequestDialog : Window
    {
        public string InputText { get; private set; }

        public StringRequestDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            InputText = InputTextBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}