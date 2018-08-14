using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlassian.Jira;
using Jankcat.VisualCompare.Lib.Models;
using Jankcat.VisualCompare.Lib.TestCaseManagers;

namespace Jankcat.VisualCompare.Lib.Utilities
{
    public class OrchestrationUtils
    {
        public static async Task RunJiraDiff(Jira jira, ITestCaseManager testCaseManager, string ticket)
        {
            // Get JIRA issue
            try
            {
                Console.WriteLine("[Orchestration][RunJiraDiff][{0}] Connecting to JIRA...", ticket);
                var issue = await JIRAUtils.GetIssueByKey(jira, ticket);
                // Get URL field
                var urls = JIRAUtils.GetUrlsField(issue);
                Console.WriteLine("[Orchestration][RunJiraDiff][{0}] URLs Retrieved from JIRA:  {1}", ticket, urls.Count);

                await testCaseManager.Browser.Initialize();

                // get all the images first
                var images = new Dictionary<string, CaptureResult>();
                foreach (var url in urls)
                {
                    var capture = await CaptureDiff(url.Value, testCaseManager);
                    images.Add(url.Key, capture);
                    Console.WriteLine("[Orchestration][RunJiraDiff][{0}] Page captured: {1}", ticket, url.Value.RightSide);
                }

                await testCaseManager.Browser.Dispose();
                Console.WriteLine("[Orchestration][RunJiraDiff][{0}] Browser Done. Pages captured: {1}", ticket, images.Count);

                // upload all the images next
                foreach (var capture in images)
                {
                    await JIRAUtils.UploadImages(issue, capture.Key, capture.Value);
                    Console.WriteLine("[Orchestration][RunJiraDiff][{0}] Page uploaded: {1}", ticket, capture.Value.URL);
                }
                Console.WriteLine("[Orchestration][RunJiraDiff][{0}] Uploads done", ticket);

                // add the comment
                await JIRAUtils.AddComment(issue, images);
                Console.WriteLine("[Orchestration][RunJiraDiff][{0}] Comment Done", ticket);
            }
            catch (Exception e)
            {
                // TODO Message someone? Alert?
                Console.WriteLine(String.Format("[Orchestration][RunJiraDiff][{0}] Exception encountered during Diff: {1}; {2}", ticket, e.Message, e.StackTrace));
                return;
            }
        }

        public static async Task<CaptureResult> CaptureDiff(UrlDetails url, ITestCaseManager testCaseManager)
        {
            var result = new CaptureResult
            {
                URL = url.RightSide
            };
            // Original Page
            await testCaseManager.GoToPage_Original(url);
            result.Original = await testCaseManager.Browser.TakeScreenshot();
            // Updated Page
            await testCaseManager.GoToPage_Updated(url);
            result.Updated = await testCaseManager.Browser.TakeScreenshot();
            // Compare Image
            var diff = ImageUtils.Compare(result.Original, result.Updated);
            result.Diff = diff.DiffImage;
            result.Difference = diff.Difference;
            return result;
        }
    }
}