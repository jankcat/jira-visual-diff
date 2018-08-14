using Jankcat.VisualCompare.Lib.Models;
using Jankcat.VisualCompare.Lib.Browsers;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace Jankcat.VisualCompare.Lib.TestCaseManagers
{
    public interface ITestCaseManager
    {
        IBrowser Browser { get; }
        bool IsActionable { get; }
        // Will happen on every navigation
        Task GoToPage_Original(UrlDetails url);
        Task GoToPage_Updated(UrlDetails url);
        void SetIssue(Issue issue);
    }
}
