using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Controls;
using System.Diagnostics;
using System.Collections.Specialized;
using Microsoft.Win32;
using System.IO;
using System.Xml.Serialization;
using OxyPlot.Axes;

namespace PLF_WPForiented.model
{
    public class AppData : INotifyPropertyChanged
    {
        public AppData()
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

            AddCommand = new RelayCommand(AddRow);
            DeleteCommand = new RelayCommand(DeleteRow);
            CopyCommand = new RelayCommand(CopyRows);
            PasteCommand = new RelayCommand(PasteRows);
            KeyDownCommand = new RelayCommand(HandleKeyDown);
            AddNewFuncCommand = new RelayCommand(AddNewFunc);
            AddInverseFuncCommand = new RelayCommand(AddInverseFunc);
            DelCurFuncCommand = new RelayCommand(DelCurFunc);
            ClearCommand = new RelayCommand(ClearDataMethod);
            SaveCommand = new RelayCommand(SaveDataMethod);
            LoadCommand = new RelayCommand(LoadDataMethod);
            PlotClickCommand = new RelayCommand(PlotView_MouseDown);
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }
        public ICommand KeyDownCommand { get; }
        public ICommand AddNewFuncCommand { get; }
        public ICommand AddInverseFuncCommand { get; }
        public ICommand DelCurFuncCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand PlotClickCommand { get; }

        #region TableData

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
                    _model.CollectionChanged += OnModelCollectionChanged;
                    SubscribeToModelPropertyChanged(_model);
                    OnPropertyChanged();
                    UpdatePlotModel();
                }
            }
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

        private void OnVertexPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsSaved = false;
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

        private void UpdatePlotModel()
        {
            FuncPlotModel.Series.Clear();
            var lineSeries = new LineSeries { Title = SelectedModelKey };

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

                    lineSeries = new LineSeries { Title = item.Key };
                    foreach (var vertex in item.Value)
                    {
                        lineSeries.Points.Add(new DataPoint(vertex.X, vertex.Y));
                    }
                    FuncPlotModel.Series.Add(lineSeries);
                }
            } else
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

        private void PlotView_MouseDown(object parameter)
        {
            if (parameter is MouseButtonEventArgs e)
            {
                var plotView = e.Source as PlotView;
                if (plotView != null)
                {
                    var mousePosition = e.GetPosition(plotView);
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
               
        #region ApplicationHandling

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

        public bool IsSavedCheck()
        {
            if (!IsSaved)
            {
                MessageBoxResult result = MessageBox.Show("Your last changes were not saved. Are you sure you want to clear data?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    return false;
                }
            }
            return true;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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

        private void HandleKeyDown(object parameter)
        {
            if (parameter is DataGrid dataGrid)
            {
                if (Keyboard.IsKeyDown(Key.C) && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    CopyCommand.Execute(dataGrid);
                }
                else if (Keyboard.IsKeyDown(Key.V) && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    PasteCommand.Execute(dataGrid);
                }
            }
        }

        private void ClearDataMethod(object parameter)
        {
            if (!IsSavedCheck())
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

            _modelKeys.Clear();
            _modelKeys = new ObservableCollection<string>(_modelDictionary.Keys);

            SelectedModelKey = "default";

            OnPropertyChanged(nameof(ModelDictionary));
            OnPropertyChanged(nameof(ModelKeys));
            OnPropertyChanged(nameof(Model));

            UpdatePlotModel();
        }

        private void SaveDataMethod(object parameter)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Special File (*.spf)|*.spf",
                InitialDirectory = @"C:\Users\YourUsername\Documents"
            };

            if (saveDialog.ShowDialog() == true)
            {
                string name = saveDialog.FileName;
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

        private void LoadDataMethod(object parameter)
        {
            if (!IsSavedCheck())
            {
                return;
            }
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "Special File (*.spf)|*.spf",
                InitialDirectory = @"C:\Users\YourUsername\Documents"
            };

            if (openDialog.ShowDialog() == true)
            {
                string name = openDialog.FileName;
                MenuLoad(name);

                IsSaved = true;
                IsChecked = false;

                OnPropertyChanged(nameof(ModelDictionary));
                OnPropertyChanged(nameof(ModelKeys));
                OnPropertyChanged(nameof(Model));
                UpdatePlotModel();
            }
        }

        public void MenuLoad(string name)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary));

            ClearFunc();
            SerializableDictionary temp = new SerializableDictionary();
            try
            {;
                using (FileStream fs = new FileStream(name, FileMode.Open))
                {
                    temp = (SerializableDictionary)serializer.Deserialize(fs);
                }

                _modelDictionary.Clear();
                _modelDictionary = temp.ToDictionary();

                _modelKeys.Clear();
                _modelKeys = new ObservableCollection<string>(_modelDictionary.Keys);

                SelectedModelKey = _modelKeys.First();

                FuncPlotModel = new PlotModel { Title = SelectedModelKey };

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error during the loading of file " + name + "\n\n" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region TableFunctions
         
        private void AddRow(object parameter)
        {
            if (parameter is Vertex vertex)
            {
                int selectedIndex = Model.IndexOf(vertex);

                Vertex newVertex;
                newVertex = new Vertex(0, 0);
                newVertex.PropertyChanged += OnVertexPropertyChanged;
                if (selectedIndex < Model.Count - 1)
                {
                    Model.Insert(selectedIndex + 1, newVertex);
                }
                else
                {
                    Model.Add(newVertex);
                }

                IsSaved = false;
            }
        }

        private void DeleteRow(object parameter)
        {
            if (Model.Count <= 1)
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

        private void CopyRows(object parameter)
        {
            if (parameter is DataGrid dataGrid)
            {
                var selectedItems = dataGrid.SelectedItems.Cast<Vertex>().ToList();
                if (selectedItems.Any())
                {
                    var clipboardText = string.Join("\n", selectedItems.Select(v => $"{v.X.ToString().Trim()}\t{v.Y.ToString().Trim()}\n"));

                    Clipboard.Clear();
                    Clipboard.SetText(clipboardText);
                }
            }
        }

        private void PasteRows(object parameter)
        {
            if (parameter is DataGrid dataGrid && Clipboard.ContainsText())
            {
                string clipboardText = Clipboard.GetText();
                var rows = clipboardText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var selectedIndex = dataGrid.SelectedIndex;

                if (selectedIndex < 0)
                {
                    selectedIndex = dataGrid.Items.Count;
                }
                foreach (var row in rows)
                {
                    var columns = row.Split('\t');
                    if ((columns.Length >= 2) && double.TryParse(columns[0], out double x) && double.TryParse(columns[1], out double y))
                    {
                        Vertex newVertex;
                        newVertex = new Vertex(x, y);
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

        private void AddNewFunc(object parameter)
        {
            StringRequestDialog dialog = new StringRequestDialog();
            string name = "";
            if (dialog.ShowDialog() == true)
            {
                name = dialog.InputText;
            }

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("No function name was typed.", "Warning");
                return;
            }
            if (ModelDictionary.ContainsKey(name))
            {
                MessageBox.Show("Name " + name + " already exists. Please type something different.", "Warning");
                return;
            }

            ModelDictionary[name] = new ObservableCollection<Vertex>
            {
                new Vertex(0, 0),
                new Vertex(0, 0)
            };

            NewFunctionPostHandle(name);
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

        #endregion
    }
}