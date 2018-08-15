﻿namespace Jankcat.VisualCompare.API.Models
{
    public class UrlRequest
    {
        public UrlRequest()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        public string[] Urls { get; set; }
        public string Guid { get; set; }
    }
}
