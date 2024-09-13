using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PiecewiseLinearFunction;
using System.Windows;

namespace TestProject
{
    [TestFixture]
    public class MainWindowTests
    {
        private MainWindow _mainWindow;

        [SetUp]
        public void Setup()
        {
            _mainWindow = new MainWindow();
        }

        [Test]
        public void MainWindow_Initialization_Test()
        {
            var data = _mainWindow.Data;
            var table = _mainWindow.Table;
            var plot = _mainWindow.Plot;
            var menu = _mainWindow.Menu;

            Assert.IsNotNull(data);
            Assert.IsNotNull(table);
            Assert.IsNotNull(plot);
            Assert.IsNotNull(menu);
        }

        [Test]
        public void MainWindow_Closing_Test()
        {
            _mainWindow.Data.IsSaved = false;
            var cancelEventArgs = new System.ComponentModel.CancelEventArgs();

            var method = typeof(MainWindow).GetMethod("MainWindow_Closing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(_mainWindow, new object[] { this, cancelEventArgs });
            //_mainWindow.MainWindow_Closing(this, cancelEventArgs);

            Assert.IsTrue(cancelEventArgs.Cancel);
        }
    }
}
