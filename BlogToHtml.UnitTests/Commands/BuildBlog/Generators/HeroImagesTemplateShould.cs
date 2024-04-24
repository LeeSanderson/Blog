using System.IO.Abstractions.TestingHelpers;
using BlogToHtml.Commands.BuildBlog;
using BlogToHtml.Commands.BuildBlog.Generators;
using BlogToHtml.Commands.BuildBlog.Models;
using FluentAssertions;
using Xunit;

namespace BlogToHtml.UnitTests.Commands.BuildBlog.Generators
{
    public class HeroImagesTemplateShould
    {
        private const string TheExpectedTitle = "The Expected Title";

        [Fact]
        public void GenerateContentWithExpectedHeader()
        {
            var fileSystem = new MockFileSystem();
            var template = new HeroImagesTemplate(RazorEngineFactory.CreateRazorEngineService());
            var contentDirectory = fileSystem.DirectoryInfo.New(@"c:\Content\");
            var outputFileInfo = fileSystem.FileInfo.New(@"c:\Content\Test1.html");
            var templateContext = new TemplateContext(contentDirectory, outputFileInfo);

            var html = template.Generate(new HeroImageModel {Title = TheExpectedTitle}, templateContext);

            html.Should().Contain($"<h1>{TheExpectedTitle}</h1>");
        }
    }
}
