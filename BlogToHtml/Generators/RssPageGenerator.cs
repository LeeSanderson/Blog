using BlogToHtml.Models;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using X.Web.RSS;
using X.Web.RSS.Structure.Validators;
using X.Web.RSS.Structure;

namespace BlogToHtml.Generators
{
    internal class RssPageGenerator : SummaryContentGeneratorBase
    {
        public RssPageGenerator(GeneratorContext generatorContext) : base(generatorContext)
        {
        }

        public override async Task GenerateSummaryContentAsync()
        {
            var rssDocument = CreateRssDocument();

            var feedFileInfo = GeneratorContext.FileSystem.FileInfo.New(Path.Combine(GeneratorContext.OutputDirectory.FullName, "rss.xml"));
            var feedContent = rssDocument.ToXml();
            await using var writer = feedFileInfo.CreateText();
            await writer.WriteAsync(feedContent);
        }

        private RssDocument CreateRssDocument()
        {
            var rssDocument = new RssDocument();
            rssDocument.Channel.Generator = "Blog Builder";
            rssDocument.Channel.Copyright = "Lee Sanderson";
            rssDocument.Channel.Description = "Lee Sanderson is a software engineer and advocate of software craftsmanship. He blogs about may topics including craftsmanship, .NET and Azure";
            rssDocument.Channel.Image =
                new RssImage
                {
                    Title = "Lee Sanderson",
                    Link = new RssUrl("https://www.sixsideddice.com/index.png"),
                    Url = new RssUrl("https://www.sixsideddice.com/blog")
                };
            rssDocument.Channel.Link = new RssUrl("https://www.sixsideddice.com");
            rssDocument.Channel.LastBuildDate = DateTime.UtcNow;
            rssDocument.Channel.Link = new RssUrl("https://www.sixsideddice.com/blog");
            rssDocument.Channel.ManagingEditor = new RssPerson("Lee Sanderson", "lee@sixsideddice.com");
            rssDocument.Channel.Title = "SixSidedDice Blog";
            rssDocument.Channel.WebMaster = new RssPerson("Lee Sanderson", "lee@sixsideddice.com");

            foreach (var article in Summaries)
            {
                var url = article.OutputFileInfo!.GetFullUrl(GeneratorContext.OutputDirectory);
                var item = new RssItem
                {
                    Author = new RssEmail("lee@sixsideddice.com"),
                    Description = article.Abstract,
                    Guid = new RssGuid { IsPermaLink = true, Value = url },
                    Link = new RssUrl(url),
                    PubDate = article.PublicationDate,
                    Title = article.Title
                };


                rssDocument.Channel.Items.Add(item);
            }

            return rssDocument;
        }

    }
}
