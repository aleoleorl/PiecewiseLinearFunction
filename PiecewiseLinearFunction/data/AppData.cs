using OxyPlot;
using OxyPlot.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace PiecewiseLinearFunction.data
{
    public class AppData
    {
        public AppData()
        {
            CurrentModel = "";

            Model = new Dictionary<string, List<InfoBlock>>();
            View = new List<LabelBlock>();
            ModelNames = new ComboBox();
            IsAllShown = new CheckBox();

            ActiveLabel = null;
            PreActiveLabel = null;

            Editor = new TextBox();

            ChoosedItemList = new List<int>();

            IsSaved = true;
        }

        public string CurrentModel;
        public Dictionary<string, List<InfoBlock>> Model;
        public List<LabelBlock> View;

        public ComboBox ModelNames;
        public CheckBox IsAllShown;
        public bool IsShown;

        public ScrollViewer TableScrollViewer;
        public StackPanel TableStackPanel;

        public Label ActiveLabel;
        public Label PreActiveLabel;

        public Canvas AppCanvas;
        public Canvas TableCanvas;
        public Canvas Item;

        public TextBox Editor;

        public Point? MouseDownPoint;
        public Point MouseUpPoint;

        public List<int> ChoosedItemList;

        public PlotModel FuncPlotModel;
        public PlotView FuncPlotView;

        public bool IsSaved;
    }
}