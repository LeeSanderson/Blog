using System;
using Xunit;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using BlogToHtml.Commands.GenerateHeroImage;
using FluentAssertions;

namespace BlogToHtml.UnitTests.Commands.GenerateHeroImage;

public class GenerateHeroImageCommandHandlerShould
{
    [Fact]
    public async Task CreateAnImage()
    {
        var mockFileSystem = new MockFileSystem();
        var contentDirectory = mockFileSystem.DirectoryInfo.New(@"c:\Content\");
        var options = new GenerateHeroImageOptions
            {OutputFileName = Path.Join(contentDirectory.FullName, "Test.png"), Title = "Test"};
        var commandHandler = new GenerateHeroImageCommandHandler(mockFileSystem, options);

        var result = await commandHandler.RunAsync();
        if (result != 0)
        {
            throw new Exception($"Content generation failed with error code {result}");
        }

        contentDirectory.Refresh();
        contentDirectory.GetFiles().First(f => f.Name == "Test.png").Should().NotBeNull();
    }
}