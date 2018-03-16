namespace CM.QA.Tools.VisualCompare.Lib.Models
{
    public class CropDimension
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public CropDimension()
        {
            Y = 0;
            X = 0;
            Width = 0;
            Height = 0;
        }
    }
}
