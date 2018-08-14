using System;
using Jankcat.VisualCompare.Lib.Models;
using Jankcat.VisualCompare.Lib.Browsers;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace Jankcat.VisualCompare.Lib.TestCaseManagers
{
    public class DefaultTestCaseManager : ITestCaseManager
    {
        public IBrowser Browser { get { return _browser; } }
        private IBrowser _browser;
        private Issue _issue;

        public string ProdBaseUrl { get; set; }
        public string StageBaseUrl { get; set; }

        public DefaultTestCaseManager(IBrowser browser)
        {
            _browser = browser;
        }

        public void SetIssue(Issue issue)
        {
            _issue = issue;
        }

        public bool IsActionable => true;

        public async Task GoToPage_Original(UrlDetails url)
        {
            await Browser.GoToPage(String.Format("{0}{1}", ProdBaseUrl, url.RightSide));
        }

        public async Task GoToPage_Updated(UrlDetails url)
        {
            await Browser.GoToPage(String.Format("{0}{1}", StageBaseUrl, url.RightSide));
        }
    }
}
