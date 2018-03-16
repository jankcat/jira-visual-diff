using System;
using CM.QA.Tools.VisualCompare.Lib.Models;
using CM.QA.Tools.VisualCompare.Lib.Utilities;
using OpenQA.Selenium;

namespace CM.QA.Tools.VisualCompare.Lib.TestCaseManagers
{
    public interface ITestCaseManager
    {
        BrowserManager Browser { get; } 
        // Will happen on every navigation
        void GoToPage_Original(UrlDetails url);
        void GoToPage_Updated(UrlDetails url);
        void Dispose();
    }
}
