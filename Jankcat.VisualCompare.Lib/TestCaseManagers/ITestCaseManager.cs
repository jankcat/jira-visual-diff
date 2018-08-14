using Jankcat.VisualCompare.Lib.Models;
using Jankcat.VisualCompare.Lib.Browsers;
using System.Threading.Tasks;

namespace Jankcat.VisualCompare.Lib.TestCaseManagers
{
    public interface ITestCaseManager
    {
        IBrowser Browser { get; } 
        // Will happen on every navigation
        Task GoToPage_Original(UrlDetails url);
        Task GoToPage_Updated(UrlDetails url);
    }
}
