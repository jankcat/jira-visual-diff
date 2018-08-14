using System;
using OpenQA.Selenium;

namespace Jankcat.VisualCompare.Lib.Browsers
{
    public class SauceLabsBrowser : SeleniumBrowser
    {
        public SauceLabsBrowser(DriverOptions opts, string grid) : base(opts, grid)
        {
        }

        public static DriverOptions GetDefaultBrowserOptions(bool tunnel  = true)
        {
            var opts = SeleniumBrowser.GetDefaultBrowserOptions();
            // If you want to disable the Sauce Connect Proxy, do so in the docker compose yml as well
            if (tunnel) opts.AddAdditionalCapability("tunnelIdentifier", "visual-jira-pool");
            opts.AddAdditionalCapability("recordVideo", false);
            opts.AddAdditionalCapability("recordScreenshots", false);
            opts.AddAdditionalCapability("screenResolution", "1920x1080");
            return opts;
        }
    }
}
