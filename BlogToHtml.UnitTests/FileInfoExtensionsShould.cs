namespace BlogToHtml.UnitTests
{
    using System.IO.Abstractions.TestingHelpers;
    using FluentAssertions;
    using Xunit;

    public class FileInfoExtensionsShould
    {
        [Theory]
        [InlineData(@"c:\Output\Blog\", @"c:\Output\Blog\SubFolder\Article.html", "./SubFolder/Article.html")]
        [InlineData(@"c:\Output\Blog", @"c:\Output\Blog\SubFolder\Article.html", "./SubFolder/Article.html")]
        [InlineData(@"c:\Output\Blog", @"c:\Output\Blog\SubFolder\SubFolder2\Article.html", "./SubFolder/SubFolder2/Article.html")]
        public void ReturnExpectedRelativeUrlForFile(string rootPath, string filePath, string expectedRelativeUrl)
        {
            var fileSystem = new MockFileSystem();
            var rootDirectory = fileSystem.DirectoryInfo.New(rootPath);
            var file = fileSystem.FileInfo.New(filePath);

            file.RelativeUrlTo(rootDirectory).Should().Be(expectedRelativeUrl);
        }

        [Theory]
        [InlineData(@"c:\Output\Blog\", @"c:\Output\Blog\SubFolder\Article.html", "https://www.sixsideddice.com/Blog/SubFolder/Article.html")]
        [InlineData(@"c:\Output\Blog", @"c:\Output\Blog\SubFolder\Article.html", "https://www.sixsideddice.com/Blog/SubFolder/Article.html")]
        [InlineData(@"c:\Output\Blog", @"c:\Output\Blog\SubFolder\SubFolder2\Article.html", "https://www.sixsideddice.com/Blog/SubFolder/SubFolder2/Article.html")]
        public void ReturnExpectedUrlForFile(string rootPath, string filePath, string expectedRelativeUrl)
        {
            var fileSystem = new MockFileSystem();
            var rootDirectory = fileSystem.DirectoryInfo.New(rootPath);
            var file = fileSystem.FileInfo.New(filePath);

            file.GetFullUrl(rootDirectory).Should().Be(expectedRelativeUrl);
        }
    }
}
