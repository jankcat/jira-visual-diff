using System;
using Jankcat.VisualCompare.Lib.Models;
using Jankcat.VisualCompare.Lib.Browsers;

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

        public void Dispose()
        {
            _browser.Dispose();
        }

        public void GoToPage_Original(UrlDetails url)
        {
            GoToPage(url);
        }

        public void GoToPage_Updated(UrlDetails url)
        {
            GoToPage(url);
        }

        private void GoToPage(UrlDetails url) 
        {
            // Do things like decide what environment, add credentials, et.
            _browser.GoToPage(String.Format("http://example.com"));
            throw new NotImplementedException();
        }
    }
}
