using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using PiecewiseLinearFunction.data;
using System.Windows;

namespace PiecewiseLinearFunction.support
{
    public class DragDropManipulator : MouseManipulator
    {
        private LineSeries LineSerie;
        private DataPoint? SelectedPoint;
        private Dictionary<string, List<InfoBlock>> DataDictionary;
        private string ModelKey;
        public delegate void DataHandler(int index);
        public event DataHandler? DataHandlerNotify;

        public DragDropManipulator(IPlotView plotView, Dictionary<string, List<InfoBlock>> dataDictionary, string modelKey, DataHandler dataHandler) : base(plotView)
        {
            this.LineSerie = plotView.ActualModel.Series[0] as LineSeries;
            this.DataDictionary = dataDictionary;
            this.ModelKey = modelKey;
            DataHandlerNotify += dataHandler;
        }

        public override void Started(OxyMouseEventArgs e)
        {
            base.Started(e);
            var hitResult = this.LineSerie.GetNearestPoint(e.Position, false);
            if (hitResult != null)
            {
                this.SelectedPoint = hitResult.DataPoint;
            }
        }

        public override void Delta(OxyMouseEventArgs e)
        {
            base.Delta(e);
            if (this.SelectedPoint.HasValue)
            {
                var xAxis = this.PlotView.ActualModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Bottom);
                var yAxis = this.PlotView.ActualModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Left);
                var screenPosition = new ScreenPoint(e.Position.X, e.Position.Y);
                var newPosition = new DataPoint(xAxis.InverseTransform(screenPosition.X), yAxis.InverseTransform(screenPosition.Y));

                var index = this.LineSerie.Points.IndexOf(this.SelectedPoint.Value);
                if (index >= 0)
                {
                    this.LineSerie.Points[index] = newPosition;

                    if (this.DataDictionary.ContainsKey(this.ModelKey))
                    {
                        var infoBlock = this.DataDictionary[this.ModelKey][index];
                        infoBlock.A = newPosition.X;
                        infoBlock.B = newPosition.Y;
                        DataHandlerNotify?.Invoke(index);
                    }
                    else
                    {
                        MessageBox.Show($"The key '{this.ModelKey}' was not found in the dictionary.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                this.PlotView.InvalidatePlot(false);
            }
        }

        public override void Completed(OxyMouseEventArgs e)
        {
            base.Completed(e);
            this.SelectedPoint = null;
        }
    }
}