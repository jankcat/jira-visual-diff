using System;
using System.Threading.Tasks;
using ImageMagick;
using PuppeteerSharp;

namespace Jankcat.VisualCompare.Lib.Browsers
{
    public class PuppeteerBrowser : IBrowser
    {
        private Browser _browser;
        private Page _page;
        private ViewPortOptions _vpOpts;

        public PuppeteerBrowser(ViewPortOptions viewPortOptions = null)
        {
            _vpOpts = viewPortOptions ?? new ViewPortOptions
            {
                Width = 1920,
                Height = 1080
            };
        }

        public async Task Initialize()
        {

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            _browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            _page = await _browser.NewPageAsync();
            await _page.SetViewportAsync(_vpOpts);
        }

        public async Task  Dispose()
        {
            await _page.CloseAsync();
            await _browser.CloseAsync();
        }

        public async Task GoToPage(string url)
        {
            await _page.GoToAsync(url);
        }

        public async Task<IMagickImage> TakeScreenshot()
        {
            IMagickImage screenshot;
            using (var stream = await _page.ScreenshotStreamAsync())
            {
                screenshot = new MagickImage(stream);
            }
            return screenshot;
        }
    }
}
