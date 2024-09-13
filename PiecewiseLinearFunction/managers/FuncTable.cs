using PiecewiseLinearFunction.data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PiecewiseLinearFunction.managers
{
    public class FuncTable
    {
        private MainWindow MWindow;
        private AppData Data;

        public FuncTable(MainWindow mWindow)
        {
            MWindow = mWindow;
            Data = MWindow.Data;
            Init();
        }

        private void Init()
        {
            Data.TableCanvas = new Canvas();
            Canvas.SetLeft(Data.TableCanvas, 10);
            Canvas.SetTop(Data.TableCanvas, 10);
            Data.TableCanvas.Width = 250;
            Data.TableCanvas.Height = 380;
            Data.TableCanvas.Background = new SolidColorBrush(Colors.LemonChiffon);
            Data.TableCanvas.MouseLeftButtonDown += Item_MouseLeftButtonDown;
            Data.TableCanvas.Clip = new RectangleGeometry(new Rect(0, 0, Data.TableCanvas.Width, Data.TableCanvas.Height));
            Data.AppCanvas.Children.Add(Data.TableCanvas);

            Data.Item = new Canvas();
            Canvas.SetLeft(Data.Item, 0);
            Canvas.SetTop(Data.Item, 70);
            Data.Item.Width = 250;
            Data.Item.Height = 310;
            Data.Item.Background = new SolidColorBrush(Colors.LemonChiffon);
            Data.Item.MouseLeftButtonDown += Item_MouseLeftButtonDown;
            Data.Item.Clip = new RectangleGeometry(new Rect(0, 0, Data.Item.Width, Data.Item.Height));
            Data.TableCanvas.Children.Add(Data.Item);

            Data.TableStackPanel = new StackPanel();
            Data.TableStackPanel.MouseDown += StackPanel_MouseDown;
            Data.TableStackPanel.PreviewMouseUp += StackPanel_MouseUp;

            Data.TableScrollViewer = new ScrollViewer();
            Data.TableScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            Data.TableScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            Data.TableScrollViewer.Width = Data.Item.Width;
            Data.TableScrollViewer.Height = Data.Item.Height - 40;
            Data.Item.Children.Add(Data.TableScrollViewer);

            Data.TableScrollViewer.MouseUp += ScrollViewer_MouseUp;
            Data.TableScrollViewer.Content = Data.TableStackPanel;
            Data.TableScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            Canvas.SetTop(Data.TableScrollViewer, 40);
            Canvas.SetLeft(Data.TableScrollViewer, 0);

            Label temp;
            temp = new Label { Width = 100, Height = 30, Margin = new Thickness(5) };
            Canvas.SetLeft(temp, 2);
            Data.Item.Children.Add(temp);
            temp.Content = "x";

            temp = new Label { Width = 100, Height = 30, Margin = new Thickness(5) };
            Canvas.SetLeft(temp, 102);
            Data.Item.Children.Add(temp);
            temp.Content = "y";

            Data.CurrentModel = "default";
            Data.Model.Add(Data.CurrentModel, new List<Vertex>());
            TableSample(Data.Model[Data.CurrentModel]);

            Data.Editor.Width = 100;
            Data.Editor.Height = 30;
            Data.Editor.Background = new SolidColorBrush(Colors.White);
            Data.Editor.Visibility = Visibility.Hidden;
            Canvas.SetZIndex(Data.Editor, 100000);
            Data.Editor.PreviewTextInput += Tb_PreviewTextInput;
            Data.Editor.TextChanged += Tb_TextChanged;
            Data.Item.Children.Add(Data.Editor);

            Data.AppCanvas.MouseLeftButtonDown += ParentCanvas_MouseLeftButtonDown;

            MWindow.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, CopyExecuted));
            MWindow.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, PasteExecuted));
        }

        private void ParentCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearTableData();
        }

        private void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ClearTableData();
        }

        #region TableDataHandler

        public void TableSample(List<Vertex> currModel)
        {
            currModel.Add(new Vertex(0, 0));
            AddRow();
            currModel.Add(new Vertex(0, 0));
            AddRow();
        }

        public void ClearTableData()
        {
            Data.ActiveLabel = null;
            Data.PreActiveLabel = null;
            Data.Editor.Visibility = Visibility.Hidden;
            if (Data.View.Count > 0)
            {
                while (Data.ChoosedItemList.Count > 0)
                {
                    Data.View[Data.ChoosedItemList[0]].A.Background = new SolidColorBrush(Colors.LightPink);
                    Data.View[Data.ChoosedItemList[0]].B.Background = new SolidColorBrush(Colors.LightPink);
                    Data.ChoosedItemList.RemoveAt(0);
                }
            }
        }

        #endregion

        #region SelectDataHandler
        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChooseItemsPreCheck(e.GetPosition((UIElement)sender));
        }

        private void ChooseItemsPreCheck(Point item)
        {
            Data.MouseDownPoint = item;
        }

        private void StackPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Data.MouseUpPoint = e.GetPosition((UIElement)sender);
            ChooseItemsCheck();
        }

        private void ChooseItemsCheck()
        {
            if (!Data.MouseDownPoint.HasValue)
            {
                return;
            }
            Data.ChoosedItemList = new List<int>();

            foreach (UIElement child in Data.TableStackPanel.Children)
            {
                if (child is not StackPanel row)
                {
                    continue;
                }
                Point rowPosition = row.TransformToAncestor(Data.TableStackPanel).Transform(new Point(0, 0));

                double rowTop = rowPosition.Y;
                double rowBottom = rowTop + row.ActualHeight;
                if (rowBottom < rowTop)
                {
                    double temp = rowBottom;
                    rowBottom = rowTop;
                    rowTop = temp;
                }

                if (IsPointInRange(Data.MouseDownPoint.Value.Y, Data.MouseUpPoint.Y, rowTop, rowBottom))
                {
                    int n = Data.View.FindIndex(item => item.Row == row);
                    if (n != -1)
                    {
                        Data.View[n].A.Background = new SolidColorBrush(Colors.LightBlue);
                        Data.View[n].B.Background = new SolidColorBrush(Colors.LightBlue);

                        if (Data.ChoosedItemList.FindIndex(item => item == n) == -1)
                        {
                            Data.ChoosedItemList.Add(n);
                        }
                    }
                }
                else
                {
                    int n = Data.View.FindIndex(item => item.Row == row);
                    if (n != -1)
                    {
                        Data.View[n].A.Background = new SolidColorBrush(Colors.LightPink);
                        Data.View[n].B.Background = new SolidColorBrush(Colors.LightPink);
                    }
                }

            }
            Data.ChoosedItemList.Sort();
            Data.MouseDownPoint = null;
        }

        private bool IsPointInRange(double start, double end, double top, double bottom)
        {
            return (start <= bottom && end >= top) || (end <= bottom && start >= top) || (start <= top && end >= bottom);
        }

        #endregion

        #region ScrollHandler

        private void ScrollViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!Data.MouseDownPoint.HasValue)
            {
                ClearTableData();
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            TableTextBoxShiftControl();
        }

        private void TableTextBoxShiftControl()
        {
            if (Data.ActiveLabel != null)
            {
                Point labelPosition = Data.ActiveLabel.TransformToAncestor(Data.Item).Transform(new Point(0, 0));

                double offsetX = 0;
                double offsetY = 0;
                Canvas.SetLeft(Data.Editor, labelPosition.X + offsetX);
                Canvas.SetTop(Data.Editor, labelPosition.Y + offsetY);

                if (labelPosition.Y - 10 < 10 && Data.Editor.Visibility == Visibility.Visible)
                {
                    Data.Editor.Visibility = Visibility.Hidden;
                }
                if (labelPosition.Y - 10 >= 10 && Data.Editor.Visibility == Visibility.Hidden)
                {
                    Data.Editor.Visibility = Visibility.Visible;
                }
            }
        }

        #endregion

        #region EditorDataControl
        private void Tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex(@"^[0-9,\-+]*$");
            return regex.IsMatch(text);
        }

        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            DoubleValueControl();
            MWindow.Plot.PlotDataChanged();
        }

        private void DoubleValueControl()
        {
            if (Data.Editor.Text.Length > 0)
            {
                //clear forbidden symbols
                string txt = Data.Editor.Text;
                int index = 0;
                while (index < txt.Length)
                {
                    if (!IsTextAllowed(txt[index].ToString()))
                    {
                        txt = txt.Remove(index, 1);
                        continue;
                    }
                    index++;
                }
                //clear all '-' except the first one
                do
                {
                    index = txt.IndexOf('-', 1);
                    if (index > 0)
                    {
                        txt = txt.Remove(index, 1);
                    }
                } while (index > 0);
                //clear all "," except the first one
                char symbol = ',';
                index = txt.IndexOf(symbol);

                if (index != -1)
                {
                    txt = txt.Substring(0, index + 1) + txt.Substring(index + 1).Replace(symbol.ToString(), string.Empty);
                }

                Data.Editor.Text = txt;
            }
            Data.ActiveLabel.Content = Data.Editor.Text;
            int n = Data.View.FindIndex(item => item.A == Data.ActiveLabel || item.B == Data.ActiveLabel);
            if (ReferenceEquals(Data.View[n].A, Data.ActiveLabel))
            {
                Data.Model[Data.CurrentModel][n].X = Data.Editor.Text != "" && Data.Editor.Text != "-" ? double.Parse(Data.Editor.Text) : 0;
            }
            else
            {
                Data.Model[Data.CurrentModel][n].Y = Data.Editor.Text != "" && Data.Editor.Text != "-" ? double.Parse(Data.Editor.Text) : 0;
            }
        }

        #endregion

        #region RowHandling
        public void AddRow(List<Vertex> currModel = null, int position = -1)
        {
            LabelBlock temp = new LabelBlock();

            temp.Row = new StackPanel { Orientation = Orientation.Horizontal };

            temp.A = new Label { Width = 100, Height = 30, Margin = new Thickness(3) };
            temp.A.Background = new SolidColorBrush(Colors.LightPink);
            if (position != -1)
            {
                temp.A.Content = currModel[position].X;
            }
            else
            {
                temp.A.Content = "0";
            }
            Canvas.SetTop(temp.A, 0);
            Canvas.SetLeft(temp.A, 2);
            temp.A.PreviewMouseUp += A_MouseUp;
            temp.A.PreviewMouseDown += A_MouseDown;
            temp.Row.Children.Add(temp.A);

            temp.B = new Label { Width = 100, Height = 30, Margin = new Thickness(3) };
            temp.B.Background = new SolidColorBrush(Colors.LightPink);
            if (position != -1)
            {
                temp.B.Content = currModel[position].Y;
            }
            else
            {
                temp.B.Content = "0";
            }
            Canvas.SetTop(temp.B, 0);
            Canvas.SetLeft(temp.B, 102);
            temp.B.MouseDown += B_MouseDown;
            temp.B.MouseUp += B_MouseUp;
            temp.Row.Children.Add(temp.B);

            temp.Plus = new Button();
            Canvas.SetTop(temp.Plus, 0);
            Canvas.SetLeft(temp.Plus, 204);
            temp.Plus.Width = 15;
            temp.Plus.Height = 15;
            temp.Plus.Content = "+";
            temp.Plus.Click += Plus_Click;
            temp.Row.Children.Add(temp.Plus);

            temp.Minus = new Button();
            Canvas.SetTop(temp.Plus, 0);
            Canvas.SetLeft(temp.Plus, 221);
            temp.Minus.Width = 15;
            temp.Minus.Height = 15;
            temp.Minus.Content = "-";
            temp.Minus.Click += Minus_Click;
            temp.Row.Children.Add(temp.Minus);

            Canvas.SetTop(temp.Row, 0);
            Canvas.SetLeft(temp.Row, 10);
            if (position != -1)
            {
                Data.TableStackPanel.Children.Insert(position, temp.Row);
                Data.View.Insert(position, temp);
            }
            else
            {
                Data.TableStackPanel.Children.Add(temp.Row);
                Data.View.Add(temp);
            }
        }

        private void A_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TableCellViewPreHandle((Label)sender);
        }

        private void A_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TableCellViewHandle((Label)sender);
            e.Handled = true;
        }

        private void B_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TableCellViewPreHandle((Label)sender);
        }

        private void B_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TableCellViewHandle((Label)sender);
            e.Handled = true;
        }

        private void TableCellViewPreHandle(Label lbl)
        {
            Data.PreActiveLabel = lbl;
        }

        private void TableCellViewHandle(Label lbl)
        {
            if (!ReferenceEquals(Data.PreActiveLabel, lbl))
            {
                Data.PreActiveLabel = null;
                return;
            }
            Data.ActiveLabel = lbl;

            int n = Data.View.FindIndex(item => item.A == Data.ActiveLabel || item.B == Data.ActiveLabel);

            Point labelPosition = Data.ActiveLabel.TransformToAncestor(Data.Item).Transform(new Point(0, 0));
            Canvas.SetLeft(Data.Editor, labelPosition.X);
            Canvas.SetTop(Data.Editor, labelPosition.Y);

            Data.Editor.Text = Data.ActiveLabel.Content.ToString();
            Data.Editor.Visibility = Visibility.Visible;
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            TableItemInclude((Button)sender);
            MWindow.Plot.PlotDataChanged();
        }

        private void TableItemInclude(Button btn)
        {
            int n = Data.View.FindIndex(item => item.Plus == btn);
            if (n == -1)
            {
                return;
            }
            else if (n == Data.View.Count - 1)
            {
                Data.Model[Data.CurrentModel].Add(new Vertex(0, 0));
                AddRow();
            }
            else
            {
                Data.Model[Data.CurrentModel].Insert(n + 1, new Vertex(0, 0));
                AddRow(Data.Model[Data.CurrentModel], n + 1);
            }
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            if (Data.View.Count > 1)
            {
                TableItemRemove((Button)sender);
                MWindow.Plot.PlotDataChanged();
            }
        }

        private void TableItemRemove(Button btn)
        {
            int n = Data.View.FindIndex(item => item.Minus == btn);
            if (n == -1)
            {
                return;
            }
            else
            {
                Data.Model[Data.CurrentModel].RemoveAt(n);

                Data.TableStackPanel.Children.Remove(Data.View[n].Row);
                Data.View.RemoveAt(n);

                for (int i = 0; i < Data.Model[Data.CurrentModel].Count; i++)
                {
                    Data.View[i].A.Content = Data.Model[Data.CurrentModel][i].X;
                    Data.View[i].B.Content = Data.Model[Data.CurrentModel][i].Y;
                }
            }
        }

        #endregion

        #region ClipboardHandling
        private void CopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ClipboardPrepareCopy();
        }

        private void ClipboardPrepareCopy()
        {
            StringBuilder clipboardData = new StringBuilder();
            for (int i = 0; i < Data.ChoosedItemList.Count; i++)
            {
                clipboardData.AppendLine($"{Data.Model[Data.CurrentModel][Data.ChoosedItemList[i]].X}\t{Data.Model[Data.CurrentModel][Data.ChoosedItemList[i]].Y}");
            }

            DataObject dataObject = new DataObject();
            dataObject.SetText(clipboardData.ToString(), TextDataFormat.Text);
            Clipboard.SetDataObject(dataObject, true);
        }

        private void PasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ClipboardPreparePaste();
            MWindow.Plot.PlotDataChanged();
        }

        private void ClipboardPreparePaste()
        {
            string clipboardData = Clipboard.GetText();
            string[] lines = clipboardData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int n = -1;
            if (Data.ChoosedItemList.Count != 0)
            {
                n = Data.ChoosedItemList[0];
            }

            foreach (string line in lines)
            {
                string[] parts = line.Split('\t');
                if (parts.Length == 2 && double.TryParse(parts[0], out double a) && double.TryParse(parts[1], out double b))
                {
                    TableInjectData(a, b, n);
                }

                if (n != -1)
                {
                    n++;
                    if (n == Data.View.Count)
                    {
                        n = -1;
                    }
                }
            }
        }

        private void TableInjectData(double a, double b, int n = -1)
        {
            if (n == -1)
            {
                Data.Model[Data.CurrentModel].Add(new Vertex(a, b));
                List<Vertex> temp = Data.Model[Data.CurrentModel];
                AddRow(temp, temp.Count - 1);
            }
            else
            {
                Data.Model[Data.CurrentModel][n].X = a;
                Data.Model[Data.CurrentModel][n].Y = b;
                Data.View[n].A.Content = a.ToString();
                Data.View[n].B.Content = b.ToString();
            }
        }

        #endregion
    }
}