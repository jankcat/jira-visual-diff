using System;
using OpenQA.Selenium;
using CM.QA.Tools.VisualCompare.Lib.Models;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;

namespace CM.QA.Tools.VisualCompare.Lib.Utilities
{
    public static class SeleniumUtil
    {

        // TODO https://github.com/zinserjan/wdio-screenshot/blob/master/src/modules/makeViewportScreenshot.js
        // TODO https://github.com/zinserjan/wdio-screenshot/blob/master/src/modules/makeElementScreenshot.js
        // TODO Browsers other than Chrome, like iOS, scaled displays, etc.

        public static IMagickImage GetFullPageScreenshot(IWebDriver driver, ScreenshotOptions options = null)
        {
            var opts = (options == null) ? new ScreenshotOptions() : options;

            // Hide Scroll Bars, Trigger Resize Event, Scroll to top
            ToggleScrollBars(driver, false);
            TriggerJSResizeEvent(driver);
            // TODO Hide/Remove elements if told
            NormalScroll(driver, 0, 0);

            // Let the browser catch up
            Task.Delay(200).Wait();

            // Get dimensions
            var dimensions = GetPageDimensions(driver);
            var shot = MakeAreaScreenshot(driver, 0, 0, dimensions.PageWidth, dimensions.PageHeight, opts);


            // Show Scrollbars again
            // TODO UnHide/UnRemove elements if told
            ToggleScrollBars(driver, true);

            return shot;
        }

        private static IMagickImage MakeAreaScreenshot(IWebDriver driver, int startX, int startY, int endX, int endY, ScreenshotOptions options)
        {
            var dimensions = GetPageDimensions(driver);
            var strat = new ScrollStrategy(dimensions);
            strat.SetScrollArea(startX, startY, endX, endY);

            // Set page height
            if (!options.SkipPageHeightSetting) SetPageHeight(driver, dimensions.PageHeight);

            // Outside list is Y, inside list is X
            var shots = new Dictionary<int, Dictionary<int, IMagickImage>>();

            var loop = false;
            do
            {
                // Scroll to the next position
                var scrollPosition = strat.GetScrollPosition();
                VirtualScroll(driver, scrollPosition.X, scrollPosition.Y);
                Task.Delay(100).Wait();

                // Take screenshot
                var screenshot = ScreenshotToImage(((ITakesScreenshot)driver).GetScreenshot());
                var cropDimensions = strat.GetCropDimensions();
                var croppedImage = ImageUtils.Crop(screenshot, cropDimensions);

                // Store screenshot
                if (!shots.ContainsKey(scrollPosition.IndexY)) shots.Add(scrollPosition.IndexY, new Dictionary<int, IMagickImage>());
                shots[scrollPosition.IndexY].Add(scrollPosition.IndexX, screenshot);

                // See if we have more shots to take, and move to them in our manager if we do
                loop = strat.HasNextScrollPosition();
                strat.MoveToNextScrollPosition();
            } while (loop);

            // Reset page height
            if (!options.SkipPageHeightSetting) SetPageHeight(driver);

            // Reset virtual scroll
            VirtualScroll(driver, 0, 0, true);


            // Merge the shots
            return ImageUtils.MergeImageLists(shots);
        }

        private static Dimensions GetPageDimensions(IWebDriver driver)
        {
            // Get Page Height
            var bodyScrollHeight = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return  document.body.scrollHeight");
            var bodyOffsetHeight = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return  document.body.offsetHeight");
            var htmlClientHeight = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return  document.documentElement.clientHeight");
            var htmlScrollHeight = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return  document.documentElement.scrollHeight");
            var htmlOffsetHeight = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return  document.documentElement.offsetHeight");
            var height = Enumerable.Max(new[] { bodyScrollHeight, bodyOffsetHeight, htmlClientHeight, htmlScrollHeight, htmlOffsetHeight });

            // Get Viewport Width
            var windowInnerWidth = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return window.innerWidth");
            var htmlClientWidth = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return  document.documentElement.clientWidth");
            var vpWidth = Enumerable.Max(new[] { windowInnerWidth, htmlClientWidth, 0 });

            // Get Viewport Height
            var windowInnerHeight = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return window.innerHeight");
            var vpHeight = Enumerable.Max(new[] { windowInnerHeight, htmlClientHeight, 0 });

            // Create output object
            var dimensions = new Dimensions
            {
                // Page Size
                PageWidth = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return document.documentElement.scrollWidth"), //documentElement.scrollWidth is used by wdio //body.offsetWidth
                PageHeight = height,
                // Get the size of the viewport
                ViewportWidth = vpWidth,
                ViewportHeight = vpHeight 
            };
            return dimensions;
        }

        private static IMagickImage ScreenshotToImage(Screenshot screenshot)
        {
            // TODO can this be reduced to new MagickImage(screenshot.AsByteArray);?
            IMagickImage screenshotImage;
            using (var memStream = new MemoryStream(screenshot.AsByteArray))
            {
                screenshotImage = new MagickImage(memStream);
            }
            return screenshotImage;
        }

        private static void VirtualScroll(IWebDriver driver, int x, int y, bool reset = false)
        {
            var script = String.Format(@"
                const w = {0} === 0 ? 0 : -1 * {0};
                const h = {1} === 0 ? 0 : -1 * {1};

                const translate = {2} ? 'none' : `translate(${{w}}px,${{h}}px)`;
                const html = document.documentElement;

                html.style.webkitTransform = translate;
                html.style.mozTransform = translate;
                html.style.msTransform = translate;
                html.style.oTransform = translate;
                html.style.transform = translate;", x, y, reset.ToString().ToLower());
            ((IJavaScriptExecutor)driver).ExecuteScript(script);
        }

        private static void NormalScroll(IWebDriver driver, int x, int y)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript(String.Format("window.scrollTo({0}, {1});", x, y));
        }

        public static void ToggleScrollBars(IWebDriver driver, bool enabled) 
        {
            if (enabled) ((IJavaScriptExecutor)driver).ExecuteScript("document.documentElement.style.overflow = '';");
            else ((IJavaScriptExecutor)driver).ExecuteScript("document.documentElement.style.overflow = 'hidden';");
        }

        public static void TriggerJSResizeEvent(IWebDriver driver)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                const evt = window.document.createEvent('UIEvents');
                evt.initUIEvent('resize', true, false, window, 0);
                window.dispatchEvent(evt);");
        }

        // See https://github.com/zinserjan/wdio-screenshot/pull/25
        public static void SetPageHeight(IWebDriver driver, int height = 0)
        {
            if (height == 0) ((IJavaScriptExecutor)driver).ExecuteScript("document.body.style.height = '';");
            else ((IJavaScriptExecutor)driver).ExecuteScript(String.Format("document.body.style.height = '{0}px';", height));
        }
    }
}
