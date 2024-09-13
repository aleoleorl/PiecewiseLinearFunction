using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using OxyPlot.Wpf;

namespace PLF_WPForiented.model
{
    public class MouseDownBehavior : Behavior<PlotView>
    {
        public static readonly DependencyProperty CommandProperty =
         DependencyProperty.Register("Command", typeof(ICommand), typeof(MouseDownBehavior), new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseDown += OnMouseDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseDown -= OnMouseDown;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Command != null && Command.CanExecute(e))
            {
                Command.Execute(e);
            }
        }
    }
}
