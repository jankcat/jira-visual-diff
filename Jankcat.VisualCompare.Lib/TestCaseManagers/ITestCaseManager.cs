using Jankcat.VisualCompare.Lib.Models;
using Jankcat.VisualCompare.Lib.Browsers;

namespace Jankcat.VisualCompare.Lib.TestCaseManagers
{
    public interface ITestCaseManager
    {
        IBrowser Browser { get; } 
        // Will happen on every navigation
        void GoToPage_Original(UrlDetails url);
        void GoToPage_Updated(UrlDetails url);
        void Dispose();
    }
}
