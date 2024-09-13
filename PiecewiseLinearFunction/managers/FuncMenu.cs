using PiecewiseLinearFunction.data;
using PiecewiseLinearFunction.support;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace PiecewiseLinearFunction.managers
{
    public class FuncMenu
    {
        private MainWindow MWindow;
        private AppData Data;

        public FuncMenu(MainWindow mWindow)
        {
            MWindow = mWindow;
            Data = MWindow.Data;
            Init();
        }

        private void Init()
        {
            Button renew = new Button();
            Canvas.SetTop(renew, 2);
            Canvas.SetLeft(renew, 2);
            renew.Width = 60;
            renew.Height = 20;
            renew.Content = "clear";
            renew.Click += Renew_Click;
            Data.TableCanvas.Children.Add(renew);

            Button save = new Button();
            Canvas.SetTop(save, 24);
            Canvas.SetLeft(save, 2);
            save.Width = 60;
            save.Height = 20;
            save.Content = "save";
            save.Click += Save_Click;
            Data.TableCanvas.Children.Add(save);

            Button load = new Button();
            Canvas.SetTop(load, 46);
            Canvas.SetLeft(load, 2);
            load.Width = 60;
            load.Height = 20;
            load.Content = "load";
            load.Click += Load_Click;
            Data.TableCanvas.Children.Add(load);

            Canvas.SetTop(Data.ModelNames, 2);
            Canvas.SetLeft(Data.ModelNames, 143);
            Data.ModelNames.Width = 102;
            Data.ModelNames.Height = 20;
            Data.ModelNames.Items.Add(Data.CurrentModel);
            Data.ModelNames.SelectedIndex = 0;
            Data.ModelNames.SelectionChanged += ModelNames_SelectionChanged;
            Data.TableCanvas.Children.Add(Data.ModelNames);

            Button addModel = new Button();
            Canvas.SetTop(addModel, 22);
            Canvas.SetLeft(addModel, 143);
            addModel.Width = 50;
            addModel.Height = 20;
            addModel.Content = "add new";
            addModel.Click += AddModel_Click;
            Data.TableCanvas.Children.Add(addModel);

            Button delModel = new Button();
            Canvas.SetTop(delModel, 22);
            Canvas.SetLeft(delModel, 195);
            delModel.Width = 50;
            delModel.Height = 20;
            delModel.Content = "del cur";
            delModel.Click += DelModel_Click;
            Data.TableCanvas.Children.Add(delModel);

            Canvas.SetTop(Data.IsAllShown, 46);
            Canvas.SetLeft(Data.IsAllShown, 143);
            Data.IsAllShown.Width = 20;
            Data.IsAllShown.Height = 20;
            Data.IsAllShown.Checked += IsAllShown_Checked;
            Data.IsAllShown.Unchecked += IsAllShown_Unchecked;
            Data.TableCanvas.Children.Add(Data.IsAllShown);
            Data.IsShown = false;

            Label lblShown = new Label();
            Canvas.SetTop(lblShown, 40);
            Canvas.SetLeft(lblShown, 160);
            lblShown.Width = 60;
            lblShown.Height = 25;
            lblShown.VerticalContentAlignment = System.Windows.VerticalAlignment.Top;
            lblShown.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            lblShown.Content = "show all";
            Data.TableCanvas.Children.Add(lblShown);

            Button inverseFunction = new Button();
            inverseFunction.Width = 102;
            inverseFunction.Height = 20;
            inverseFunction.Content = "add Inverse func";
            Canvas.SetTop(inverseFunction, 64);
            Canvas.SetLeft(inverseFunction, 143);
            inverseFunction.Click += InverseFunction_Click;
            Data.TableCanvas.Children.Add(inverseFunction);
        }

        #region FileHandler

        private void Renew_Click(object sender, RoutedEventArgs e)
        {
            if (!Data.IsSaved)
            {
                MessageBoxResult result = MessageBox.Show("Your last changes were not saved. Are you sure you want to clear data?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            MWindow.Table.ClearTableData();
            ViewClear();
            Data.Model.Clear();

            Data.CurrentModel = "default";
            Data.Model.Add(Data.CurrentModel, new List<Vertex>());
            MWindow.Table.TableSample(Data.Model[Data.CurrentModel]);

            Data.IsAllShown.IsChecked = false;

            Data.ModelNames.Items.Clear();
            Data.ModelNames.Items.Add("default");
            Data.ModelNames.SelectedIndex = 0;

            MWindow.Plot.PlotDataChanged();
        }

        private void ViewClear()
        {
            for (int i = 0; i < Data.View.Count; i++)
            {
                Data.TableStackPanel.Children.Remove(Data.View[i].Row);
            }
            Data.View.Clear();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
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
                Data.IsSaved = true;
            }
        }

        public void MenuSaveAs(string name)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary));
            using (FileStream fs = new FileStream(name, FileMode.Create))
            {
                var serializableDictionary = new SerializableDictionary(Data.Model);
                serializer.Serialize(fs, serializableDictionary);
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "Special File (*.spf)|*.spf",
                InitialDirectory = @"C:\Users\YourUsername\Documents"
            };

            if (openDialog.ShowDialog() == true)
            {
                string name = openDialog.FileName;
                MenuLoad(name);
                MWindow.Plot.PlotDataChanged();
            }
        }

        public void MenuLoad(string name)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary));
            SerializableDictionary temp = new SerializableDictionary();
            try
            {
                using (FileStream fs = new FileStream(name, FileMode.Open))
                {
                    temp = (SerializableDictionary)serializer.Deserialize(fs);
                }

                ViewClear();

                Data.Model.Clear();

                Data.Model = temp.ToDictionary();
                Data.CurrentModel = Data.Model.First().Key;

                Data.ModelNames.Items.Clear();
                foreach (var item in Data.Model)
                {
                    Data.ModelNames.Items.Add(item.Key);
                }
                Data.ModelNames.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error during the loading of file " + name + "\n\n" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region FunctionHandler
        private void ModelNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Data.ModelNames.SelectedItem == null)
            {
                return;
            }
            Data.CurrentModel = Data.ModelNames.SelectedItem.ToString();

            MWindow.Table.ClearTableData();
            ViewClear();

            for (int i = 0; i < Data.Model[Data.CurrentModel].Count; i++)
            {
                MWindow.Table.AddRow(Data.Model[Data.CurrentModel], i);
            }

            if (!Data.IsShown)
            {
                Data.FuncPlotModel.Title = Data.CurrentModel + " function";
            }
            MWindow.Plot.PlotDataChanged();
        }

        private void DelModel_Click(object sender, RoutedEventArgs e)
        {
            if (Data.Model.Count > 1)
            {
                string temp = Data.CurrentModel;
                Data.Model.Remove(Data.CurrentModel);
                Data.CurrentModel = Data.Model.First().Key;

                MWindow.Table.ClearTableData();
                ViewClear();

                Data.ModelNames.Items.Remove(temp);
                Data.ModelNames.SelectedIndex = 0;

                MWindow.Plot.PlotDataChanged();
            }
        }

        private void AddModel_Click(object sender, RoutedEventArgs e)
        {
            string tempName = ShowCustomDialog(CustomDialogType.Text, "Enter the name of function:", "Add Model");
            if (tempName != null && tempName != "")
            {
                if (Data.Model.ContainsKey(tempName))
                {
                    ShowCustomDialog(CustomDialogType.Default, $"Name {tempName} already exists. Please type something different.", "Warning");
                }
                else
                {
                    MWindow.Table.ClearTableData();
                    ViewClear();

                    Data.CurrentModel = tempName;
                    Data.Model.Add(Data.CurrentModel, new List<Vertex>());
                    MWindow.Table.TableSample(Data.Model[Data.CurrentModel]);

                    Data.ModelNames.Items.Add(Data.CurrentModel);
                    Data.ModelNames.SelectedIndex = Data.ModelNames.Items.Count - 1;

                    MWindow.Plot.PlotDataChanged();
                }
            }
            else
            {
                ShowCustomDialog(CustomDialogType.Default, "No function name was typed.", "Warning");
            }
        }

        private string ShowCustomDialog(CustomDialogType type, string message, string title = null)
        {
            Window customDialog = new Window
            {
                Title = title == null ? type.ToString() : title,
                Width = type == CustomDialogType.Text ? 400 : 400,
                Height = type == CustomDialogType.Text ? 200 : 100,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = MWindow,
                ResizeMode = ResizeMode.NoResize
            };

            StackPanel mainStackPanel = new StackPanel
            {
                Margin = new Thickness(10)
            };

            TextBlock messageTextBlock = new TextBlock
            {
                Text = message,
                Margin = new Thickness(0, 0, 0, 10)
            };
            mainStackPanel.Children.Add(messageTextBlock);

            TextBox inputTextBox = new TextBox();
            if (type == CustomDialogType.Text)
            {
                inputTextBox.Width = 350;
                inputTextBox.Height = 30;
                mainStackPanel.Children.Add(inputTextBox);
            }

            StackPanel buttonStackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            if (type == CustomDialogType.Text)
            {
                Button okButton = new Button
                {
                    Content = "OK",
                    Width = 75,
                    Margin = new Thickness(5)
                };
                okButton.Click += (sender, e) =>
                {
                    customDialog.DialogResult = true;
                    customDialog.Close();
                };
                buttonStackPanel.Children.Add(okButton);

                Button cancelButton = new Button
                {
                    Content = "Cancel",
                    Width = 75,
                    Margin = new Thickness(5)
                };
                cancelButton.Click += (sender, e) =>
                {
                    customDialog.DialogResult = false;
                    customDialog.Close();
                };
                buttonStackPanel.Children.Add(cancelButton);
            }

            mainStackPanel.Children.Add(buttonStackPanel);

            customDialog.Content = mainStackPanel;

            bool? result = customDialog.ShowDialog();

            return result == true ? inputTextBox.Text : null;
        }

        private void IsAllShown_Unchecked(object sender, RoutedEventArgs e)
        {
            Data.IsShown = false;
            Data.FuncPlotModel.Title = Data.CurrentModel + " function";
            MWindow.Plot.PlotDataChanged();
        }

        private void IsAllShown_Checked(object sender, RoutedEventArgs e)
        {
            Data.IsShown = true;
            Data.FuncPlotModel.Title = "all functions";
            MWindow.Plot.PlotDataChanged();
        }

        private void InverseFunction_Click(object sender, RoutedEventArgs e)
        {
            string name = "inv " + Data.CurrentModel;
            if (Data.Model.ContainsKey(name))
            {
                int n = 1;
                while (Data.Model.ContainsKey(name + n.ToString()))
                {
                    n++;
                }
                name += n.ToString();
            }
            MWindow.Table.ClearTableData();
            ViewClear();

            Data.Model.Add(name, new List<Vertex>());
            for (int i = 0; i < Data.Model[Data.CurrentModel].Count; i++)
            {
                Data.Model[name].Add(new Vertex(Data.Model[Data.CurrentModel][i].Y, Data.Model[Data.CurrentModel][i].X));
            }

            Data.CurrentModel = name;

            Data.ModelNames.Items.Add(Data.CurrentModel);
            Data.ModelNames.SelectedIndex = Data.ModelNames.Items.Count - 1;

            MWindow.Plot.PlotDataChanged();
        }

        #endregion
    }
}