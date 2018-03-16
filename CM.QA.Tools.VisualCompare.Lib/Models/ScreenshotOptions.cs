using System;
namespace CM.QA.Tools.VisualCompare.Lib.Models
{
    public class ScreenshotOptions
    {
        public bool SkipPageHeightSetting { get; set; }

        public ScreenshotOptions() 
        {
            SkipPageHeightSetting = false;
        }
    }
}
