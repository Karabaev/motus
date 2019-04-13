namespace SerialService.Sitemap
{
    using System;

    /// <summary>
    /// Единица
    /// </summary>
    public class SitemapNode
    {
        public SitemapFrequency? Frequency { get; set; }
        public DateTime? LastModified { get; set; }
        public double? Priority { get; set; }
        public string Url { get; set; }
    }

    public enum SitemapFrequency
    {
        Never,
        Monthly,
        Daily,
    }
}