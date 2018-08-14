using System;
using System.Threading.Tasks;
using ImageMagick;
using Jankcat.VisualCompare.Lib.SeleniumSupport;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Jankcat.VisualCompare.Lib.Browsers
{
    public class SeleniumBrowser : IBrowser
    {
        private readonly IWebDriver _driver;
        public IWebDriver Driver { get { return _driver; } }

        public SeleniumBrowser(DriverOptions opts, string grid)
        {
            _driver = new RemoteWebDriver(new Uri(grid), opts);
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

        public static DriverOptions GetDefaultBrowserOptions()
        {
            var opts = new ChromeOptions();
            opts.AddAdditionalCapability("platform", "Windows 10");
            opts.AddAdditionalCapability("version", "latest");
            opts.AddAdditionalCapability("idleTimeout", 300);
            opts.AddAdditionalCapability("name", "Visual Test");
            return opts;
        }

        public static DriverOptions AddTestName(DriverOptions opts, string name)
        {
            opts.AddAdditionalCapability("name", name);
            return opts;
        }

        public static DriverOptions AddCredentials(DriverOptions opts, string username, string accessKey)
        {
            // May need to be overridden for BrowserStackBrowser
            opts.AddAdditionalCapability("username", username);
            opts.AddAdditionalCapability("accessKey", accessKey);
            return opts;
        }

        public IMagickImage TakeScreenshot()
        {
            // Hide Scroll Bars, Trigger Resize Event, Scroll to top
            SeleniumUtils.ToggleScrollBars(_driver, false);
            SeleniumUtils.TriggerJSResizeEvent(_driver);
            // TODO Hide/Remove elements if told
            SeleniumUtils.NormalScroll(_driver, 0, 0);

            // Let the browser catch up
            Task.Delay(200).Wait();

            var seOpts = new ScreenshotOptions();
            var dimensions = SeleniumUtils.GetPageDimensions(_driver);
            var shot = SeleniumUtils.MakeAreaScreenshot(_driver, 0, 0, dimensions.PageWidth, dimensions.PageHeight, seOpts);

            // Show Scrollbars again
            // TODO UnHide/UnRemove elements if told
            SeleniumUtils.ToggleScrollBars(_driver, true);

            return shot;
        }
    }
}
