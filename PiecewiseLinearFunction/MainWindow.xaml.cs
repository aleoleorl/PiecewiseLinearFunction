using PiecewiseLinearFunction.data;
using PiecewiseLinearFunction.managers;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

[assembly: InternalsVisibleTo("TestProject")]

namespace PiecewiseLinearFunction
{
    public partial class MainWindow : Window
    {
        public AppData Data;
        public FuncTable Table;
        public FuncPlot Plot;
        public FuncMenu Menu;

        public MainWindow()
        {
            InitializeComponent();

            Data = new AppData();

            AreaInit();

            Table = new FuncTable(this);
            Plot = new FuncPlot(this);
            Menu = new FuncMenu(this);
        }

        private void AreaInit()
        {
            Data.AppCanvas = new Canvas();
            Data.AppCanvas.Background = new SolidColorBrush(Colors.Transparent);
            Data.AppCanvas.SetBinding(Canvas.WidthProperty, new Binding("ActualWidth") { Source = this });
            Data.AppCanvas.SetBinding(Canvas.HeightProperty, new Binding("ActualHeight") { Source = this });
            this.Content = Data.AppCanvas;

            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Data.IsSaved)
            {
                MessageBoxResult result = MessageBox.Show("Your last changes were not saved. Are you sure you want to close the application?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}