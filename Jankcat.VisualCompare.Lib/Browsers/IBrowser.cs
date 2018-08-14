using System.Threading.Tasks;
using ImageMagick;

namespace Jankcat.VisualCompare.Lib.Browsers
{
    public interface IBrowser
    {
        Task Dispose();
        Task Initialize();
        Task GoToPage(string url);
        Task<IMagickImage> TakeScreenshot();
    }
}