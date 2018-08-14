using Atlassian.Jira;
using Jankcat.VisualCompare.Lib.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jankcat.VisualCompare.Lib.Utilities
{
    public static class JIRAUtils
    {
        public static Jira Create(string url, string username = null, string password = null)
        {
            return Jira.CreateRestClient(url, username, password);
        }

        public static async Task<Issue> GetIssueByKey(Jira jira, string key)
        {
            return await jira.Issues.GetIssueAsync(key);
        }

        public static Dictionary<string, UrlDetails> GetUrlsField(Issue issue)
        {
            // Get URL Field
            if (issue["URLs"] == null) throw new Exception(String.Format("URLs field does not exist for issue: {0}", issue.Key));
            var urlsField = issue["URLs"].ToString();
            var urls = urlsField.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            // Remove base url
            var endpoints = new Dictionary<string, UrlDetails>();
            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            // Cycle through each line in the URL field
            var count = 0;
            foreach (var url in urls)
            {
                count++;
                if (String.IsNullOrWhiteSpace(url)) continue;
                var uri = new Uri(url);
                var strip = String.Format("{0}://{1}", uri.Scheme, uri.Authority);
                var details = new UrlDetails
                {
                    LeftSide = strip,
                    RightSide = url.Replace(strip, "")
            };
                endpoints.Add(String.Format("{0}-URL{1}", timestamp, count), details);
            }
            return endpoints;
        }

        public static async Task AddComment(Issue issue, Dictionary<string, CaptureResult> pages)
        {
            var comment = "";
            foreach (var page in pages)
            {
                var commentColor = (page.Value.Difference <= 1.0) ? "#59afe1" : "#f79232";
                var singleComment = String.Format(@"*Page:* {0}
{{color:{2}}}% Difference: {3}{{color}}
||Prod||Stage||Diff||
|!{1}-original.png|thumbnail!|!{1}-updated.png|thumbnail!|!{1}-diff.png|thumbnail!|
", page.Value.URL, page.Key, commentColor, page.Value.Difference);
                comment += singleComment;
            }
            await issue.AddCommentAsync(comment);
        }

        public static async Task UploadImages(Issue issue, string name, CaptureResult result)
        {

            // Add the attachments to the attachment array
            // timestamp-name-image.png
            UploadAttachmentInfo[] attachments = {
                new UploadAttachmentInfo(String.Format("{0}-original.png", name), result.Original.ToByteArray(ImageMagick.MagickFormat.Png)),
                new UploadAttachmentInfo(String.Format("{0}-updated.png", name), result.Updated.ToByteArray(ImageMagick.MagickFormat.Png)),
                new UploadAttachmentInfo(String.Format("{0}-diff.png", name), result.Diff.ToByteArray(ImageMagick.MagickFormat.Png))
            };
            await issue.AddAttachmentAsync(attachments);
        }
    }
}