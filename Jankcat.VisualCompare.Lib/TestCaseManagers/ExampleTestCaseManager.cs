using System;
using Jankcat.VisualCompare.Lib.Models;
using Jankcat.VisualCompare.Lib.Browsers;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace Jankcat.VisualCompare.Lib.TestCaseManagers
{
    public class ExampleTestCaseManager : ITestCaseManager
    {
        public IBrowser Browser { get; }
        private Issue _issue;

        public ExampleTestCaseManager(IBrowser browser)
        {
            Browser = browser;
        }

        public void SetIssue(Issue issue)
        {
            _issue = issue;
        }

        public bool IsActionable => _issue.Labels.Contains("Visual-Test");

        public async Task GoToPage_Original(UrlDetails url)
        {
            if (_issue.Labels.Contains("SomeModifier")) {
                // Do something...
            }
            var originalBase = (url.LeftSide.ToLower().Contains("brandone.com")) ? origBaseUrlBrandOne : origBaseUrlBrandTwo;
            await GoToPage(String.Format("{0}{1}", originalBase, url.RightSide));
        }

        public async Task GoToPage_Updated(UrlDetails url)
        {
            var updatedBase = (url.LeftSide.ToLower().Contains("brandone.com")) ? updatedBaseUrlBrandOne : updatedBaseUrlBrandTwo;
            await GoToPage(String.Format("{0}{1}", updatedBase, url.RightSide));
        }

        private async Task GoToPage(string url)
        {
            await Browser.GoToPage(url);
        }

        const string origBaseUrlBrandTwo = "https://brandtwo.com";
        const string updatedBaseUrlBrandTwo = "https://user:pass@preprod.brandtwo.com";
        const string origBaseUrlBrandOne = "https://brandone.com";
        const string updatedBaseUrlBrandOne = "https://user:pass@preprod.brandone.com";

    }
}
