using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PLF_WPForiented.model
{
    public class Vertex : INotifyPropertyChanged
    {
        private double _x;
        private double _y;

        public double X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Vertex(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public Vertex()
        {

        }

        public override string ToString()
        {
            return $"{X}\t{Y}";
        }
    }
}