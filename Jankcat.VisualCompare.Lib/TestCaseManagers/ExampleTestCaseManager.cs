using System;
using Jankcat.VisualCompare.Lib.Models;
using Jankcat.VisualCompare.Lib.Browsers;
using System.Threading.Tasks;

namespace Jankcat.VisualCompare.Lib.TestCaseManagers
{
    public class ExampleTestCaseManager : ITestCaseManager
    {
        public IBrowser Browser { get; }

        public ExampleTestCaseManager(IBrowser browser)
        {
            Browser = browser;
        }

        public async Task GoToPage_Original(UrlDetails url)
        {
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
