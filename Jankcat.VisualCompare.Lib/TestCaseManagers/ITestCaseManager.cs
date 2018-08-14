using System;
using Jankcat.VisualCompare.Lib.Models;
using Jankcat.VisualCompare.Lib.Utilities;
using OpenQA.Selenium;

namespace Jankcat.VisualCompare.Lib.TestCaseManagers
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
