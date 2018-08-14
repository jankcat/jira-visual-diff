using Jankcat.VisualCompare.Lib.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jankcat.VisualCompare.Lib.TestCaseManagers;

namespace Jankcat.VisualCompare.Lib.Utilities
{
    public static class NNAHUtils
    {
        public static async Task RunJiraDiff(string ticket)
        {
            // Get JIRA issue
            try
            {
                Console.WriteLine("[NNAHUtils][RunJiraDiff][{0}] Connecting to JIRA...", ticket);
                var jiraHost = Environment.GetEnvironmentVariable("VISDIFF_JIRA_HOST");
                var jiraUser = Environment.GetEnvironmentVariable("VISDIFF_JIRA_USER");
                var jiraKey = Environment.GetEnvironmentVariable("VISDIFF_JIRA_KEY");
                var jira = JIRAUtils.Create(jiraHost, jiraUser, jiraKey);
                var issue = await JIRAUtils.GetIssueByKey(jira, ticket);
                // Get URL field
                var urls = JIRAUtils.GetUrlsField(issue);
                Console.WriteLine("[NNAHUtils][RunJiraDiff][{0}] URLs Retrieved from JIRA:  {1}", ticket, urls.Count);

                // get all the images first
                var images = new Dictionary<string, CaptureResult>();
                foreach (var url in urls)
                {
                    // Get test case manager
                    ITestCaseManager testCaseManager;
                    if (ticket.StartsWith("COOL-", StringComparison.InvariantCultureIgnoreCase)) testCaseManager = new ExampleTestCaseManager(CreateCMBrowser());
                    //else if (ticket.StartsWith("NOTCOOL-", StringComparison.InvariantCultureIgnoreCase)) testCaseManager = new SomeOtherTestCaseManager(CreateCMBrowser());
                    else testCaseManager = new DefaultTestCaseManager(CreateCMBrowser());
                    // capture the diff
                    var capture = OrchestrationUtils.CaptureDiff(url.Value, testCaseManager);
                    testCaseManager.Dispose();
                    images.Add(url.Key, capture);
                    Console.WriteLine("[NNAHUtils][RunJiraDiff][{0}] Page captured: {1}", ticket, url.Value.RightSide);
                }
                Console.WriteLine("[NNAHUtils][RunJiraDiff][{0}] Browser Done. Pages captured: {1}", ticket, images.Count);

                // upload all the images next
                foreach (var capture in images)
                {
                    await JIRAUtils.UploadImages(issue, capture.Key, capture.Value);
                    Console.WriteLine("[NNAHUtils][RunJiraDiff][{0}] Page uploaded: {1}", ticket, capture.Value.URL);
                }
                Console.WriteLine("[NNAHUtils][RunJiraDiff][{0}] Uploads done", ticket);

                // add the comment
                await JIRAUtils.AddComment(issue, images);
                Console.WriteLine("[NNAHUtils][RunJiraDiff][{0}] Comment Done", ticket);
            }
            catch (Exception e)
            {
                // Message tim!
                Console.WriteLine(String.Format("[NNAHUtils][RunJiraDiff][{0}] Exception encountered during Diff: {1}; {2}", ticket, e.Message, e.StackTrace));
                return;
            }
        }

        private static BrowserManager CreateCMBrowser()
        {
            var user = Environment.GetEnvironmentVariable("VISDIFF_GRID_USER");
            var apiKey = Environment.GetEnvironmentVariable("VISDIFF_GRID_KEY");
            var host = Environment.GetEnvironmentVariable("VISDIFF_GRID_HOST");

            var caps = BrowserManager.GetDefaultCapabilities();
            caps = BrowserManager.AddCredentials(caps, user, apiKey);
            return new BrowserManager(caps, host);
        }
    }
}
