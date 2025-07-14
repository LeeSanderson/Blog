using BlogToHtml.Generators;
using BlogToHtml.Models;
using FluentAssertions;
using NSubstitute;
using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Net.Http;
using System.Threading.Tasks;
using X.Web.RSS;
using X.Web.RSS.Structure;
using X.Web.RSS.Structure.Validators;
using Xunit;

namespace BlogToHtml.UnitTests.Generators;

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
                PublicationDate = new DateTime(2024, 07, 02, 06, 54, 0),
                PublicationStatus = PublicationStatus.Published,
                OutputFileInfo = fileSystem.FileInfo.New(@"c:\Output\Categry\Article.html")
            }
        };

        rssPageGenerator.OnArticleGenerated(articles[0]);

        await rssPageGenerator.GenerateSummaryContentAsync();
        var outputDirectory = fileSystem.DirectoryInfo.New(@"c:\Output\");
        var expectedRssDocument = CreateExpectedRssDocument(outputDirectory, articles);
        var actualRssDocument = RssDocument.Load(await fileSystem.File.ReadAllTextAsync(@"c:\Output\rss.xml"));

        actualRssDocument
            .Should()
            .BeEquivalentTo(
                expectedRssDocument,
                options => options.Excluding(rss => rss.Channel.Language));
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
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var generatorContext = 
            new GeneratorContext(
                RazorEngineFactory.CreateRazorEngineService(), 
                fileSystem,
                contentDirectory, 
                outputDirectory,
                httpClientFactory);
        var generator = new RssPageGenerator(generatorContext);
        return generator;
    }

    private static DateTime UtcTimeWithoutMilliseconds()
    {
        var date = DateTime.UtcNow;
        return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Kind);
    }
}