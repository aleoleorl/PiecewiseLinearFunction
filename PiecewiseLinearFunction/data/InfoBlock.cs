namespace PiecewiseLinearFunction.data
{
    [Serializable]
    public class InfoBlock
    {
        public InfoBlock(double A, double B)
        {
            this.A = A;
            this.B = B;
        }
        public InfoBlock()
        {

        }
        public double A { get; set; }
        public double B { get; set; }
    }
}