using System;
using Jankcat.VisualCompare.Lib.Models;
using Jankcat.VisualCompare.Lib.Browsers;
using System.Threading.Tasks;

namespace Jankcat.VisualCompare.Lib.TestCaseManagers
{
    public class DefaultTestCaseManager : ITestCaseManager
    {
        public IBrowser Browser { get { return _browser; } }
        private IBrowser _browser;
        public DefaultTestCaseManager(IBrowser browser)
        {
            _browser = browser;
        }

        public async Task GoToPage_Original(UrlDetails url)
        {
            await GoToPage(url);
        }

        public async Task GoToPage_Updated(UrlDetails url)
        {
            await GoToPage(url);
        }

        private async Task GoToPage(UrlDetails url) 
        {
            // Do things like decide what environment, add credentials, et.
            await _browser.GoToPage(String.Format("http://example.com"));
            throw new NotImplementedException();
        }
    }
}
