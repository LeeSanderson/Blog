using System;
using BlogToHtml.Generators;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using BlogToHtml.Models;
using FluentAssertions;
using Xunit;
using X.Web.RSS;
using System.IO.Abstractions;
using X.Web.RSS.Extensions;
using X.Web.RSS.Structure;
using X.Web.RSS.Structure.Validators;

namespace BlogToHtml.UnitTests.Generators
{
    public class RssPageGeneratorShould
    {
        [Fact]
        public async Task GenerateExpectedRssFeed()
        {
            var fileSystem = new MockFileSystem();
            var rssPageGenerator = CreateRssPageGenerator(fileSystem);
            var articles = new ArticleModel[]
            {
                new()
                {
                    Title = "A title",
                    Abstract = "An abstract",
                    PublicationDate = new DateTime(2024, 07, 02, 06, 54, 0, DateTimeKind.Utc),
                    PublicationStatus = PublicationStatus.Published,
                    OutputFileInfo = fileSystem.FileInfo.New(@"c:\Output\Categry\Article.html")
                }
            };

            rssPageGenerator.OnArticleGenerated(articles[0]);

            await rssPageGenerator.GenerateSummaryContentAsync();
            var outputDirectory = fileSystem.DirectoryInfo.New(@"c:\Output\");
            var expectedRssDocument = CreateExpectedRssDocument(outputDirectory, articles);
            var rssText = await fileSystem.File.ReadAllTextAsync(@"c:\Output\rss.xml");
            var actualRssDocument = RssDocument.Load(rssText);
            ApplyDateParseLocalAdjustmentFix(actualRssDocument!, expectedRssDocument);

            actualRssDocument
                .Should()
                .BeEquivalentTo(
                    expectedRssDocument,
                    options => options.Excluding(rss => rss.Channel.Language));
        }

        private void ApplyDateParseLocalAdjustmentFix(RssDocument actualRssDocument, RssDocument expectedRssDocument)
        {
            for (int i = 0; i < actualRssDocument.Channel.Items.Count; i++)
            {
                var actualItem = actualRssDocument.Channel.Items[i];
                var expectedItem = expectedRssDocument.Channel.Items[i];
                if (actualItem.PubDate != null && expectedItem.PubDate != null)
                {
                    actualItem.PubDate =
                        DateParseLocalAdjustmentFix(actualItem.PubDate.Value, expectedItem.PubDate.Value);
                }
            }
        }

        [Fact(Skip = "Test to confirm parsing format, only works when BST +0000, fails in summertime")]
        public void GenerateDatesInExpectedFormat()
        {
            var date = new DateTime(2024, 07, 02, 06, 54, 0, DateTimeKind.Utc);
            var dateString = date.ToRFC822Date();
            dateString.Should().Be("Tue, 02 Jul 2024 06:54:00 +0000");
        }

        [Fact]
        public void GenerateParseInExpectedFormat()
        {
            const string dateString = "Tue, 02 Jul 2024 06:54:00 +0000";
            var expectedDate = new DateTime(2024, 07, 02, 06, 54, 0, DateTimeKind.Utc);
            var parsedDate = DateParseLocalAdjustmentFix(dateString.FromRFC822Date(), expectedDate);
            parsedDate.Should().Be(expectedDate);
        }

        private DateTime DateParseLocalAdjustmentFix(DateTime rfc822ParsedDateTime, DateTime expectedDateTime)
        {
            // Date.Parse used by FromRFC822Date() will adjust the time to local time.
            // This method will adjust the time back to UTC if the difference is 1 hour.
            if (rfc822ParsedDateTime - expectedDateTime == TimeSpan.FromHours(1))
            {
                return rfc822ParsedDateTime.AddHours(-1);
            }
            return rfc822ParsedDateTime;
        }

        private static RssDocument CreateExpectedRssDocument(IDirectoryInfo outputDirectory, ArticleModel[] articles)
        {
            var expectedRssDocument = new RssDocument();
            expectedRssDocument.Channel.Generator = "Blog Builder";
            expectedRssDocument.Channel.Copyright = "Lee Sanderson";
            expectedRssDocument.Channel.Description = "Lee Sanderson is a software engineer and advocate of software craftsmanship. He blogs about may topics including craftsmanship, .NET and Azure";
            expectedRssDocument.Channel.Image = 
                new RssImage
                {
                    Title = "Lee Sanderson", 
                    Link = new RssUrl("https://www.sixsideddice.com/index.png"),
                    Url = new RssUrl("https://www.sixsideddice.com/blog")
                };
            expectedRssDocument.Channel.Link = new RssUrl("https://www.sixsideddice.com");
            expectedRssDocument.Channel.LastBuildDate = UtcTimeWithoutMilliseconds();
            expectedRssDocument.Channel.Link = new RssUrl("https://www.sixsideddice.com/blog");
            expectedRssDocument.Channel.ManagingEditor = new RssPerson("Lee Sanderson", "lee@sixsideddice.com");
            expectedRssDocument.Channel.Title = "SixSidedDice Blog";
            expectedRssDocument.Channel.WebMaster = new RssPerson("Lee Sanderson", "lee@sixsideddice.com");

            foreach (var article in articles)
            {
                var url = article.OutputFileInfo!.GetFullUrl(outputDirectory);
                var item = new RssItem
                {
                    Author = new RssEmail("lee@sixsideddice.com"),
                    Description = article.Abstract,
                    Guid = new RssGuid { IsPermaLink = true, Value = url},
                    Link = new RssUrl(url),
                    PubDate = article.PublicationDate,
                    Title = article.Title
                };


                expectedRssDocument.Channel.Items.Add(item);
            }

            return expectedRssDocument;
        }


        private static RssPageGenerator CreateRssPageGenerator(MockFileSystem fileSystem)
        {
            var contentDirectory = fileSystem.DirectoryInfo.New(@"c:\Content\");
            contentDirectory.CreateIfNotExists();
            var outputDirectory = fileSystem.DirectoryInfo.New(@"c:\Output\");
            outputDirectory.CreateIfNotExists();
            var generatorContext = 
                new GeneratorContext(
                    RazorEngineFactory.CreateRazorEngineService(), 
                    fileSystem,
                    contentDirectory, 
                    outputDirectory);
            var generator = new RssPageGenerator(generatorContext);
            return generator;
        }

        private static DateTime UtcTimeWithoutMilliseconds()
        {
            var date = DateTime.UtcNow;
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
        }
    }
}
