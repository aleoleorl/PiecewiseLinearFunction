namespace PiecewiseLinearFunction.data
{
    [Serializable]
    public class Vertex
    {
        public Vertex(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public Vertex()
        {

        }
        public double X { get; set; }
        public double Y { get; set; }
    }
}