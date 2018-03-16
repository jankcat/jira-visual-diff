using System;
using CM.QA.Tools.VisualCompare.Lib.TestCaseManagers;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace CM.QA.Tools.VisualCompare.Lib.Utilities
{
    public class BrowserManager
    {
        // TODO Sauce connect proxy?

        private IWebDriver _driver;
        public IWebDriver Driver { get { return _driver; } }

        // Public Factories
        public BrowserManager(DesiredCapabilities caps, string grid)
        {
            _driver = new RemoteWebDriver(new Uri(grid), caps, TimeSpan.FromSeconds(600));
        }

        public static DesiredCapabilities GetDefaultCapabilities()
        {
            DesiredCapabilities caps = new DesiredCapabilities();
            caps.SetCapability("browserName", "Chrome");
            caps.SetCapability("version", "latest");
            caps.SetCapability("platform", "Windows 10");
            caps.SetCapability("recordVideo", false);
            caps.SetCapability("recordScreenshots", false);
            caps.SetCapability("screenResolution", "1920x1080");
            caps.SetCapability("idleTimeout", 300);
            caps.SetCapability("name", "Visual Test");
            caps.SetCapability("tunnelIdentifier", "visual-jira-pool");
            return caps;
            // If you want to use BrowserStack instead of Sauce, you'll want to change these capabilities
            // If you want to disable the Sauce Connect Proxy, do so in the docker compose yml, and comment out the tunnelIdentifier capability in this file
        }

        public static DesiredCapabilities AddTestName(DesiredCapabilities caps, string name)
        {
            caps.SetCapability("name", name);
            return caps;
        }

        public static DesiredCapabilities AddCredentials(DesiredCapabilities caps, string username, string accessKey)
        {   
            caps.SetCapability("username", username);
            caps.SetCapability("accessKey", accessKey);
            return caps;
            // If you want to use BrowserStack instead of Sauce, you'll want to change these capabilities
        }

        public void Dispose()
        {
            _driver.Quit();
        }

        public void GoToPage(string url)
        {
            _driver.Navigate().GoToUrl(url);
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(90.00));
            wait.Until(driver => ((IJavaScriptExecutor)_driver).ExecuteScript("return document.readyState").Equals("complete"));
        }
    }
}
