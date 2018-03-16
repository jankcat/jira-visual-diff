﻿using System;
using CM.QA.Tools.VisualCompare.Lib.Models;
using CM.QA.Tools.VisualCompare.Lib.Utilities;

namespace CM.QA.Tools.VisualCompare.Lib.TestCaseManagers
{
    public class DefaultTestCaseManager : ITestCaseManager
    {
        public BrowserManager Browser { get { return _browser; } }
        private BrowserManager _browser;
        public DefaultTestCaseManager(BrowserManager browser)
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
