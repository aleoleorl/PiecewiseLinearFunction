using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using DynamicData;
using OxyPlot;
using OxyPlot.Avalonia;
using OxyPlot.Axes;
using PLF_AvaloniaOriented.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Windows.Input;

namespace PLF_AvaloniaOriented.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public MainWindowViewModel()
        {
            InitFunc();

            _modelDictionary = new Dictionary<string, ObservableCollection<Vertex>>
            {
                { "default", _model }
            };

            _modelKeys = new ObservableCollection<string>(_modelDictionary.Keys);

            SelectedModelKey = "default";

            FuncPlotModel = new PlotModel { Title = "default function" };
            UpdatePlotModel();

            AddCommand = new RelayCommand<object>(AddRow);
            DeleteCommand = new RelayCommand<object>(DeleteRow);
            KeyDownCommand = ReactiveCommand.Create<object>(HandleKeyDown);
            CopyCommand = new RelayCommand<object>(CopyRows);
            PasteCommand = new RelayCommand<object>(PasteRows);
            AddNewFuncCommand = new RelayCommand(AddNewFunc);
            DelCurFuncCommand = new RelayCommand(DelCurFunc);
            AddInverseFuncCommand = new RelayCommand(AddInverseFunc);
            ClearCommand = new RelayCommand(ClearDataMethod);
            SaveCommand = new RelayCommand(async _ => await SaveDataMethod()); 
            LoadCommand = new RelayCommand(LoadDataMethod);
            PlotClickCommand = new RelayCommand<object>(PlotView_MouseDown);
        }

        #region DataAndCommands

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand KeyDownCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand AddNewFuncCommand { get; }
        public ICommand DelCurFuncCommand { get; }
        public ICommand AddInverseFuncCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand PlotClickCommand { get; }

        public ObservableCollection<Vertex> _model;
        public ObservableCollection<Vertex> Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    if (_model != null)
                    {
                        _model.CollectionChanged -= OnModelCollectionChanged;
                        UnsubscribeFromModelPropertyChanged(_model);
                    }
                    _model = value;
                    if (_model != null)
                    {
                        _model.CollectionChanged += OnModelCollectionChanged;
                        SubscribeToModelPropertyChanged(_model);
                    }
                    OnPropertyChanged();
                    UpdatePlotModel();
                }
            }
        }

        private Dictionary<string, ObservableCollection<Vertex>> _modelDictionary;
        public Dictionary<string, ObservableCollection<Vertex>> ModelDictionary
        {
            get => _modelDictionary;
            set
            {
                if (_modelDictionary != value)
                {
                    _modelDictionary = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedModelKey;
        public string SelectedModelKey
        {
            get => _selectedModelKey;
            set
            {
                if (_selectedModelKey != value)
                {
                    _selectedModelKey = value;
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(_selectedModelKey) && _modelDictionary.ContainsKey(_selectedModelKey))
                    {
                        Model = _modelDictionary[_selectedModelKey];
                    }
                }
            }
        }

        private ObservableCollection<string> _modelKeys;
        public ObservableCollection<string> ModelKeys
        {
            get => _modelKeys;
            set
            {
                if (_modelKeys != value)
                {
                    _modelKeys = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void ClearFunc()
        {
            if (_model != null)
            {
                foreach (var item in _model)
                {
                    item.PropertyChanged -= OnVertexPropertyChanged;
                }
                _model.CollectionChanged -= OnModelCollectionChanged;
                _model.Clear();
            }
        }

        private void InitFunc()
        {
            ClearFunc();
            _model = new ObservableCollection<Vertex>();
            Vertex vertex;
            vertex = new Vertex(0, 0);
            vertex.PropertyChanged += OnVertexPropertyChanged;
            _model.Add(vertex);
            vertex = new Vertex(0, 0);
            vertex.PropertyChanged += OnVertexPropertyChanged;
            _model.Add(vertex);

            _model.CollectionChanged += OnModelCollectionChanged;
        }

        private void OnModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Vertex vertex in e.NewItems)
                {
                    vertex.PropertyChanged += OnVertexPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (Vertex vertex in e.OldItems)
                {
                    vertex.PropertyChanged -= OnVertexPropertyChanged;
                }
            }

            UpdatePlotModel();
        }

        private void SubscribeToModelPropertyChanged(ObservableCollection<Vertex> model)
        {
            foreach (var vertex in model)
            {
                vertex.PropertyChanged += OnVertexPropertyChanged;
            }
        }

        private void UnsubscribeFromModelPropertyChanged(ObservableCollection<Vertex> model)
        {
            foreach (var vertex in model)
            {
                vertex.PropertyChanged -= OnVertexPropertyChanged;
            }
        }

        private void OnVertexPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsSaved = false;
            UpdatePlotModel();
        }

        #endregion

        #region AppHandling

        private bool _isSaved = true;
        public bool IsSaved
        {
            get => _isSaved;
            set
            {
                if (_isSaved != value)
                {
                    _isSaved = value;
                }
            }
        }

        public async Task<bool> IsSavedCheck()
        {
            if (!IsSaved)
            {
                var msgWnd = new MessageBoxWindow("Your last changes were not saved. Are you sure you want to clear data?", "Warning", "YES", "NO");
                bool result = await msgWnd.ShowDialog<bool>(AppData.Instance.Wnd);
                return result;
            }
            return true;
        }

        private async void ClearDataMethod(object parameter)
        {
            if (!await IsSavedCheck())
            {
                return;
            }

            InitFunc();

            IsSaved = true;
            IsChecked = false;

            _modelDictionary.Clear();
            _modelDictionary = new Dictionary<string, ObservableCollection<Vertex>>
            {
                { "default", _model }
            };

            ModelKeys.Clear();
            ModelKeys = new ObservableCollection<string>(_modelDictionary.Keys);

            SelectedModelKey = "default";

            OnPropertyChanged(nameof(ModelDictionary));
            OnPropertyChanged(nameof(ModelKeys));
            OnPropertyChanged(nameof(Model));

            UpdatePlotModel();
        }

        private async Task SaveDataMethod()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save Special File",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Special File (*.spd)", Extensions = new List<string> { "spf" } }
                },
                InitialFileName = "document.spf",
                Directory = documentsPath
            };

            var result = await saveFileDialog.ShowAsync(AppData.Instance.Wnd);

            if (result != null)
            {
                string name = result;
                MenuSaveAs(name);
                IsSaved = true;
            }
        }

        public void MenuSaveAs(string name)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary));
            using (FileStream fs = new FileStream(name, FileMode.Create))
            {
                var serializableDictionary = new SerializableDictionary(ModelDictionary);
                serializer.Serialize(fs, serializableDictionary);
            }
        }

        private async void LoadDataMethod(object parameter)
        {
            if (!await IsSavedCheck())
            {
                return;
            }
            var openDialog = new OpenFileDialog
            {
                Title = "Open Special File",
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Special File (*.spf)", Extensions = new List<string> { "spf" } }
                },
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            var result = await openDialog.ShowAsync(AppData.Instance.Wnd);

            if (result != null && result.Length > 0)
            {
                string name = result[0];
                MenuLoad(name);

                IsSaved = true;
                IsChecked = false;

                OnPropertyChanged(nameof(ModelDictionary));
                OnPropertyChanged(nameof(ModelKeys));
                OnPropertyChanged(nameof(Model));
                UpdatePlotModel();
            }
        }

        public async void MenuLoad(string name)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary));

            ClearFunc();
            SerializableDictionary temp = new SerializableDictionary();
            try
            {
                using (FileStream fs = new FileStream(name, FileMode.Open))
                {
                    temp = (SerializableDictionary)serializer.Deserialize(fs);
                }

                _modelDictionary.Clear();
                _modelDictionary = temp.ToDictionary();

                ModelKeys.Clear();
                ModelKeys = new ObservableCollection<string>(_modelDictionary.Keys);

                SelectedModelKey = _modelKeys.First();

                FuncPlotModel = new PlotModel { Title = SelectedModelKey };

            }
            catch (Exception ex)
            {
                var msgWnd = new MessageBoxWindow("An error during the loading of file " + name + "\n\n" + ex, "Error");
                await msgWnd.ShowDialog<bool>(AppData.Instance.Wnd);
            }
        }

        #endregion

        #region TableHandling

        private void AddRow(object parameter)
        {
            Vertex item = parameter as Vertex;
            if (item == null)
            {
                return;
            }
            var index = Model.IndexOf(item);
            Dispatcher.UIThread.Invoke(() =>
            {
                Vertex newVertex;
                newVertex = new Vertex(0, 0);
                newVertex.PropertyChanged += OnVertexPropertyChanged;
                Model.Insert(index + 1, newVertex);
            });
            IsSaved = false;
        }

        private void DeleteRow(object parameter)
        {
            if (parameter == null || Model.Count <= 1)
            {
                return;
            }
            if (parameter is Vertex vertex)
            {
                vertex.PropertyChanged -= OnVertexPropertyChanged;
                Model.Remove(vertex);

                IsSaved = false;
            }
        }

        private void HandleKeyDown(object parameter)
        {
            if (parameter is KeyEventArgs e)
            {
                if (e.Key == Key.C && e.KeyModifiers.HasFlag(KeyModifiers.Control))
                {
                    CopyCommand.Execute(null);
                }
                else if (e.Key == Key.V && e.KeyModifiers.HasFlag(KeyModifiers.Control))
                {
                    PasteCommand.Execute(null);
                }
            }
        }

        private async void CopyRows(object parameter)
        {
            var mainWindow = AppData.Instance.Wnd as MainWindow;
            if (mainWindow == null)
            {
                return;
            }
            var dataGrid = mainWindow.FindControl<DataGrid>("dataGrid");
            if (dataGrid.SelectedItems != null && dataGrid.SelectedItems.Cast<object>().Any())
            {
                var clipboardText = new StringBuilder();
                foreach (var item in dataGrid.SelectedItems)
                {
                    if (item is Vertex vertex)
                    {
                        clipboardText.AppendLine($"{vertex.X}\t{vertex.Y}");
                    }
                }
                var topLevel = TopLevel.GetTopLevel(dataGrid);
                if (topLevel != null)
                {
                    var textToCopy = clipboardText.ToString().Replace("\"", string.Empty);
                    await topLevel.Clipboard.SetTextAsync(textToCopy);
                }
            }
        }

        private async void PasteRows(object parameter)
        {
            var mainWindow = AppData.Instance.Wnd as MainWindow;
            if (mainWindow == null)
            {
                return;
            }
            var dataGrid = mainWindow.FindControl<DataGrid>("dataGrid");
            var topLevel = TopLevel.GetTopLevel(dataGrid);
            if (topLevel != null)
            {
                string clipboardText = await topLevel.Clipboard.GetTextAsync();
                if (!string.IsNullOrEmpty(clipboardText))
                {
                    var rows = clipboardText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    var selectedIndex = dataGrid.SelectedIndex;

                    var itemsSource = dataGrid.ItemsSource as IList<Vertex>;
                    if (itemsSource == null)
                    {
                        return;
                    }
                    if (selectedIndex < 0)
                    {
                        selectedIndex = itemsSource.Count;
                    }
                    foreach (var row in rows)
                    {
                        var columns = row.Split('\t');
                        if ((columns.Length >= 2) && double.TryParse(columns[0], out double x) && double.TryParse(columns[1], out double y))
                        {
                            Vertex newVertex = new Vertex(x, y);
                            newVertex.PropertyChanged += OnVertexPropertyChanged;
                            if (selectedIndex < Model.Count)
                            {
                                Model.Insert(selectedIndex, newVertex);

                                Model[selectedIndex + 1].PropertyChanged -= OnVertexPropertyChanged;
                                Model.RemoveAt(selectedIndex + 1);
                            }
                            else
                            {
                                Model.Add(newVertex);
                            }
                            selectedIndex++;
                        }
                    }

                    IsSaved = false;
                }
            }
        }

        private async void AddNewFunc(object parameter)
        {
            var reqWnd = new RequestBoxWindow("Enter a name for the new function:", "Warning");
            bool result = await reqWnd.ShowDialog<bool>(AppData.Instance.Wnd);

            string name = "";
            if (result == true)
            {
                name = reqWnd.TextBoxText;
            }

            if (string.IsNullOrEmpty(name))
            {
                var messWnd = new MessageBoxWindow("No function name was typed.", "Warning");
                await messWnd.ShowDialog<bool>(AppData.Instance.Wnd);
                return;
            }
            if (ModelDictionary.ContainsKey(name))
            {
                var messWnd = new MessageBoxWindow("Name " + name + " already exists. Please type something different.", "Warning");
                await messWnd.ShowDialog<bool>(AppData.Instance.Wnd);
                return;
            }

            ModelDictionary[name] = new ObservableCollection<Vertex>
            {
                new Vertex(0, 0),
                new Vertex(0, 0)
            };
            NewFunctionPostHandle(name);
        }

        private void NewFunctionPostHandle(string name)
        {
            IsSaved = false;
            ModelKeys.Add(name);
            SelectedModelKey = name;
            OnPropertyChanged(nameof(ModelDictionary));
            OnPropertyChanged(nameof(ModelKeys));
            OnPropertyChanged(nameof(Model));
        }

        private void DelCurFunc(object parameter)
        {
            if (ModelDictionary.Count <= 1)
            {
                return;
            }

            IsSaved = false;

            ModelDictionary.Remove(SelectedModelKey);
            _modelKeys = new ObservableCollection<string>(_modelDictionary.Keys);
            SelectedModelKey = ModelKeys.First();
            OnPropertyChanged(nameof(ModelDictionary));
            OnPropertyChanged(nameof(ModelKeys));
            OnPropertyChanged(nameof(Model));
        }

        private void AddInverseFunc(object parameter)
        {
            string name = "Inv " + SelectedModelKey;
            if (ModelDictionary.ContainsKey(name))
            {
                int n = 1;
                while (ModelDictionary.ContainsKey(name + n.ToString()))
                {
                    n++;
                }
                name += n.ToString();
            }

            ModelDictionary[name] = new ObservableCollection<Vertex>();
            foreach (var vertex in Model)
            {
                double x = vertex.Y;
                double y = vertex.X;
                ModelDictionary[name].Add(new Vertex(x, y));
            }

            NewFunctionPostHandle(name);
        }

        #endregion

        #region PlotHandling

        private PlotModel _funcPlotModel;
        public PlotModel FuncPlotModel
        {
            get => _funcPlotModel;
            set
            {
                if (_funcPlotModel != value)
                {
                    _funcPlotModel = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdatePlotModel()
        {
            FuncPlotModel.Series.Clear();

            var lineSeries = new OxyPlot.Series.LineSeries { Title = SelectedModelKey };

            foreach (var vertex in _model)
            {
                lineSeries.Points.Add(new DataPoint(vertex.X, vertex.Y));
            }
            FuncPlotModel.Series.Add(lineSeries);

            if (IsChecked)
            {
                FuncPlotModel.Title = "all functions";
                foreach (var item in ModelDictionary)
                {
                    if (item.Key == SelectedModelKey)
                    {
                        continue;
                    }

                    lineSeries = new OxyPlot.Series.LineSeries { Title = item.Key };
                    foreach (var vertex in item.Value)
                    {
                        lineSeries.Points.Add(new DataPoint(vertex.X, vertex.Y));
                    }
                    FuncPlotModel.Series.Add(lineSeries);
                }
            }
            else
            {
                FuncPlotModel.Title = SelectedModelKey;
            }

            FuncPlotModel.InvalidatePlot(true);
        }

        private bool _isChecked = false;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                    UpdatePlotModel();
                }
            }
        }

        public void PlotView_MouseDown(object parameter)
        {
            dynamic pointerData = parameter;
            var sender = pointerData.Sender;
            var eventArgs = pointerData.EventArgs;

            var plotView = sender as PlotView;
            if (plotView != null)
            {
                var mousePosition = eventArgs.GetPosition(plotView);
                var plotModel = plotView.Model;
                var xAxis = plotModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Bottom);
                var yAxis = plotModel.Axes.FirstOrDefault(a => a.Position == AxisPosition.Left);

                if (xAxis != null && yAxis != null)
                {
                    double x = xAxis.InverseTransform(mousePosition.X);
                    double y = yAxis.InverseTransform(mousePosition.Y);

                    UpdateVertex(x, y);
                }
            }

        }

        private void UpdateVertex(double x, double y)
        {
            if (Model == null || Model.Count == 0)
                return;

            Vertex closeVertex = null;
            double minDistance = double.MaxValue;

            foreach (var vertex in Model)
            {
                double distance = Math.Sqrt(Math.Pow(vertex.X - x, 2) + Math.Pow(vertex.Y - y, 2));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closeVertex = vertex;
                }
            }

            if (closeVertex != null)
            {
                closeVertex.X = x;
                closeVertex.Y = y;
                OnPropertyChanged(nameof(Model));
            }
        }

        #endregion

    }
}