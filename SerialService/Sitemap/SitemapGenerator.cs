namespace SerialService.Sitemap
{
    using SerialService.DAL;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;
    using System.Xml.Linq;
    using System.Globalization;
    using System;

    public class SitemapGenerator
    {
        const string domian = "https://motus-cinema.ru/";
        IAppUnitOfWork uow = new AppUnitOfWork();

        public string GetSitemapDocument(IEnumerable<SitemapNode> sitemapNodes)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement root = new XElement(xmlns + "urlset");

            foreach (SitemapNode sitemapNode in sitemapNodes)
            {
                XElement urlElement = new XElement(
                    xmlns + "url",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                    sitemapNode.LastModified == null ? null : new XElement(
                        xmlns + "lastmod",
                        sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                    sitemapNode.Frequency == null ? null : new XElement(
                        xmlns + "changefreq",
                        sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                    sitemapNode.Priority == null ? null : new XElement(
                        xmlns + "priority",
                        sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                root.Add(urlElement);
            }

            XDocument document = new XDocument(root);
            return document.ToString();
        }

        public IReadOnlyCollection<SitemapNode> GetNodes(UrlHelper urlHelper)
        {
            List<SitemapNode> nodes = new List<SitemapNode>();

            nodes.Add(
                    new SitemapNode
                    {
                        Url = domian,
                        Frequency = SitemapFrequency.Daily,
                        Priority = 1.0,
                    }
                );
            nodes.Add(
                    new SitemapNode
                    {
                        Url = domian + "about",
                        Frequency = SitemapFrequency.Never,
                        Priority = 0.5                        
                    }
                );
            nodes.Add(
                    new SitemapNode
                    {
                        Url = domian + "for_holders",
                        Frequency = SitemapFrequency.Never,
                        Priority = 0.5
                    }
                );

            foreach (int id in uow.VideoMaterials.GetAllVisibleToUser().Select(vm => vm.ID))
            {
                nodes.Add(
                   new SitemapNode()
                   {
                       Url = domian + $"films/{id}",
                       Frequency = SitemapFrequency.Monthly,
                       Priority = 1.0
                   });
            }

            return nodes;
        }
    }
}