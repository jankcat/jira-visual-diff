using ImageMagick;

namespace Jankcat.VisualCompare.Lib.Browsers
{
    public interface IBrowser
    {
        void Dispose();
        void GoToPage(string url);
        IMagickImage TakeScreenshot();
    }
}
