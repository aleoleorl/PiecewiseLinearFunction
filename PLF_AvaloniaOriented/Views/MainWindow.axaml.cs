using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PLF_AvaloniaOriented.ViewModels;

namespace PLF_AvaloniaOriented.Views
{
    public partial class MainWindow : Window
    {
        private bool _canClose;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

            AppData.Instance.Wnd = this;
            this.AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
            plotView.PointerPressed += OnPlotViewPointerPressed;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            viewModel?.KeyDownCommand.Execute(e);
        }

        private void OnPlotViewPointerPressed(object sender, PointerPressedEventArgs e)
        {
            var viewModel = DataContext as MainWindowViewModel;
            var data = new { Sender = sender, EventArgs = e };
            viewModel?.PlotClickCommand.Execute(data);
        }

        protected override async void OnClosing(WindowClosingEventArgs e)
        {
            if (!_canClose && DataContext is MainWindowViewModel viewModel)
            {
                e.Cancel = true;

                bool result = await viewModel.IsSavedCheck();

                if (result)
                {
                    _canClose = true;
                    Close();
                }
            }
            base.OnClosing(e);
        }
    }
}