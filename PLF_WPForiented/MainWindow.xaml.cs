using PLF_WPForiented.model;
using System.Windows;
using System.Windows.Controls;

namespace PLF_WPForiented
{
    public partial class MainWindow : Window
    {
        public AppData AppData { get; set; }
        public MainWindow()
        {
            AppData = new AppData();
            DataContext = AppData;

            InitializeComponent();
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!AppData.IsSavedCheck())
            {
                e.Cancel = true;
            }
        }

        private void DataGrid_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            if (e.Item is Vertex vertex)
            {
                e.ClipboardRowContent.Clear();
                e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, dataGrid.Columns[0], vertex.X));
                e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, dataGrid.Columns[1], vertex.Y));
            }
        }
    }
}