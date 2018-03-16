using System;
using CM.QA.Tools.VisualCompare.Lib.Models;
using CM.QA.Tools.VisualCompare.Lib.Utilities;

namespace CM.QA.Tools.VisualCompare.Lib.TestCaseManagers
{
    public class ExampleTestCaseManager : ITestCaseManager
    {
        public BrowserManager Browser { get; }

        public ExampleTestCaseManager(BrowserManager browser)
        {
            Browser = browser;
        }

        public void Dispose()
        {
            Browser.Dispose();
        }

        public void GoToPage_Original(UrlDetails url)
        {
            var originalBase = (url.LeftSide.ToLower().Contains("brandone.com")) ? origBaseUrlBrandOne : origBaseUrlBrandTwo;
            GoToPage(String.Format("{0}{1}", originalBase, url.RightSide));
        }

        public void GoToPage_Updated(UrlDetails url)
        {
            var updatedBase = (url.LeftSide.ToLower().Contains("brandone.com")) ? updatedBaseUrlBrandOne : updatedBaseUrlBrandTwo;
            GoToPage(String.Format("{0}{1}", updatedBase, url.RightSide));
        }

        private void GoToPage(string url)
        {
            Browser.GoToPage(url);
        }

        const string origBaseUrlBrandTwo = "https://brandtwo.com";
        const string updatedBaseUrlBrandTwo = "https://user:pass@preprod.brandtwo.com";
        const string origBaseUrlBrandOne = "https://brandone.com";
        const string updatedBaseUrlBrandOne = "https://user:pass@preprod.brandone.com";

    }
}
