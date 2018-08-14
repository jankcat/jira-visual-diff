using System;
using System.Threading.Tasks;
using ImageMagick;
using PuppeteerSharp;

namespace Jankcat.VisualCompare.Lib.Browsers
{
    public class PuppeteerBrowser : IBrowser
    {
        private Browser _browser;

        public PuppeteerBrowser()
        {
        }

        public async Task Initialize()
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            _browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
        }

        public async Task  Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task GoToPage(string url)
        {
            var page = await _browser.NewPageAsync();
            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = 1920,
                Height = 1080
            });
            await page.GoToAsync(url);
        }

        public async Task<IMagickImage> TakeScreenshot()
        {
            throw new NotImplementedException();
        }
    }
}
