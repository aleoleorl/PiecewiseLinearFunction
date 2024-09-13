using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using PiecewiseLinearFunction.data;
using PiecewiseLinearFunction.support;
using System.Windows.Controls;
using System.Windows.Media;

namespace PiecewiseLinearFunction.managers
{
    public class FuncPlot
    {
        private MainWindow MWindow;
        private AppData Data;

        public FuncPlot(MainWindow mWindow)
        {
            MWindow = mWindow;
            Data = MWindow.Data;
            Init();
        }

        private void Init()
        {
            Data.FuncPlotModel = new PlotModel { Title = "default function" };

            PlotDataHandler();

            Data.FuncPlotView = new PlotView
            {
                Background = new SolidColorBrush(Colors.LemonChiffon),
                Model = Data.FuncPlotModel,
                Controller = new PlotController(),
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                Width = 250,
                Height = 250
            };

            Canvas.SetLeft(Data.FuncPlotView, 400);
            Canvas.SetTop(Data.FuncPlotView, 10);
            Data.AppCanvas.Children.Add(Data.FuncPlotView);
        }

        private void PlotDataHandler()
        {
            if (Data.FuncPlotModel.Series.Count > 0)
            {
                Data.FuncPlotModel.Series.Clear();
            }

            if (Data.IsShown)
            {
                foreach (var item in Data.Model)
                {
                    PlotPortionDataHandler(item.Key);
                }
            }
            else
            {
                PlotPortionDataHandler(Data.CurrentModel);
            }

            if (Data.FuncPlotView != null && Data.FuncPlotView.Controller != null)
            {
                Data.FuncPlotView.Controller.BindMouseDown(OxyMouseButton.Left, new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) =>
                {
                    controller.AddMouseManipulator(view, new DragDropManipulator(view, Data.Model, Data.CurrentModel, ReVertexView), args);
                }));
            }
        }

        private void PlotPortionDataHandler(string model)
        {
            var lineSeries = new LineSeries
            {
                Title = Data.CurrentModel + " function",
                MarkerType = MarkerType.Circle
            };

            foreach (var info in Data.Model[model])
            {
                lineSeries.Points.Add(new DataPoint(info.X, info.Y));
            }

            Data.FuncPlotModel.Series.Add(lineSeries);
        }

        private void ReVertexView(int index)
        {
            Data.View[index].A.Content = Data.Model[Data.CurrentModel][index].X;
            Data.View[index].B.Content = Data.Model[Data.CurrentModel][index].Y;
            Data.IsSaved = false;
        }

        public void PlotDataChanged()
        {
            PlotDataHandler();
            Data.FuncPlotModel.InvalidatePlot(true);

            Data.IsSaved = false;
        }
    }
}