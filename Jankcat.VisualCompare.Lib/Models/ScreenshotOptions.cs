using System;
namespace Jankcat.VisualCompare.Lib.Models
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
