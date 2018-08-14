using ImageMagick;

namespace Jankcat.VisualCompare.Lib.Models
{
    public class CaptureResult
    {
        public IMagickImage Original { get; set; }
        public IMagickImage Updated { get; set; }
        public IMagickImage Diff { get; set; }
        public double Difference { get; set; }
        public string URL { get; set; }
    }
}