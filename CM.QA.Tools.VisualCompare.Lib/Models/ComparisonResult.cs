using ImageMagick;

namespace CM.QA.Tools.VisualCompare.Lib.Models
{
    public class ComparisonResult
    {
        public double Difference
        {
            get
            {
                return (ChangedPixels / TotalPixels) * 100;
            }
        }
        public double ChangedPixels { get; set; }
        public double TotalPixels { get; set; }
        public IMagickImage DiffImage { get; set; }
    }
}