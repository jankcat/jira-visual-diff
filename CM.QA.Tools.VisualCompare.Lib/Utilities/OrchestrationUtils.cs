using CM.QA.Tools.VisualCompare.Lib.Models;
using CM.QA.Tools.VisualCompare.Lib.TestCaseManagers;

namespace CM.QA.Tools.VisualCompare.Lib.Utilities
{
    public class OrchestrationUtils
    {
        public static CaptureResult CaptureDiff(UrlDetails url, ITestCaseManager testCaseManager)
        {
            var result = new CaptureResult
            {
                URL = url.RightSide
            };
            // Original Page
            testCaseManager.GoToPage_Original(url);
            result.Original = SeleniumUtil.GetFullPageScreenshot(testCaseManager.Browser.Driver);
            // Updated Page
            testCaseManager.GoToPage_Updated(url);
            result.Updated = SeleniumUtil.GetFullPageScreenshot(testCaseManager.Browser.Driver);
            // Compare Image
            var diff = ImageUtils.Compare(result.Original, result.Updated);
            result.Diff = diff.DiffImage;
            result.Difference = diff.Difference;
            return result;
        }
    }
}