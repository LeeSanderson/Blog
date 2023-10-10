using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using BlogToHtml.Commands.BuildBlog;
using BlogToHtml.Commands.BuildBlog.Generators;
using FluentAssertions;
using Xunit;

namespace BlogToHtml.UnitTests.Commands.BuildBlog.Generators
{
    public class MarkdownToHtmlContentGeneratorShould
    {
        [Fact]
        public async Task GenerateExpectedHtml()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { @"c:\Content\Test1.md", new MockFileData("# This is a H1") }
            });
            var generator = CreateMarkdownToHtmlContentGenerator(fileSystem);
            var sourceFile = fileSystem.FileInfo.New(@"c:\Content\Test1.md");

            await generator.GenerateContentAsync(sourceFile);

            fileSystem
                .GetFile(@"c:\Output\Test1.html")
                .TextContents
                .Should()
                .Contain(@"<h1 id=""this-is-a-h1"">This is a H1</h1>");
        }

        [Fact]
        public async Task GenerateLinkToOtherArticle()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { @"c:\Content\Azure\Compute\VirtualMachines.md", new MockFileData("# This is about VMs") },
                { @"c:\Content\Azure\Storage\StorageAccounts.md", new MockFileData("# This is about storage of [VMs](../Compute/VirtualMachines.md)") },
            });
            var generator = CreateMarkdownToHtmlContentGenerator(fileSystem);
            var sourceFile = fileSystem.FileInfo.New(@"c:\Content\Azure\Storage\StorageAccounts.md");

            await generator.GenerateContentAsync(sourceFile);

            fileSystem
                .GetFile(@"c:\Output\Azure\Storage\StorageAccounts.html")
                .TextContents
                .Should()
                .Contain(@"<a href=""../Compute/VirtualMachines.html"">VMs</a>");
        }

        [Fact]
        public async Task ResolveLinkToAmbiguousArticleInSameFolder()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { @"c:\Content\Azure\Compute\VirtualMachines.md", new MockFileData("# This is about VMs") },
                { @"c:\Content\Azure\Compute\VMTypes.md", new MockFileData("# This is about types of [VMs](VirtualMachines.md)") },
            });
            var generator = CreateMarkdownToHtmlContentGenerator(fileSystem);
            var sourceFile = fileSystem.FileInfo.New(@"c:\Content\Azure\Compute\VMTypes.md");

            await generator.GenerateContentAsync(sourceFile);

            fileSystem
                .GetFile(@"c:\Output\Azure\Compute\VMTypes.html")
                .TextContents
                .Should()
                .Contain(@"<a href=""VirtualMachines.html"">VMs</a>");
        }

        [Fact]
        public async Task ResolveLinkToOtherAmbiguousArticle()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { @"c:\Content\Azure\Compute\VirtualMachines.md", new MockFileData("# This is about VMs") },
                { @"c:\Content\Azure\Storage\StorageAccounts.md", new MockFileData("# This is about storage of [VMs](VirtualMachines.md)") },
            });
            var generator = CreateMarkdownToHtmlContentGenerator(fileSystem);
            var sourceFile = fileSystem.FileInfo.New(@"c:\Content\Azure\Storage\StorageAccounts.md");

            await generator.GenerateContentAsync(sourceFile);

            fileSystem
                .GetFile(@"c:\Output\Azure\Storage\StorageAccounts.html")
                .TextContents
                .Should()
                .Contain(@"<a href=""../Compute/VirtualMachines.html"">VMs</a>");
        }

        [Fact]
        public async Task ResolveLinkToAmbiguousImageInSameFolder()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { @"c:\Content\Azure\Compute\VirtualMachines.md", new MockFileData("# This is about VMs ![VMs](VM.png)") },
                { @"c:\Content\Azure\Compute\VM.png", new MockFileData(new byte[] { 0x1b, 0x2b, 0x3b }) },
            });
            var generator = CreateMarkdownToHtmlContentGenerator(fileSystem);
            var sourceFile = fileSystem.FileInfo.New(@"c:\Content\Azure\Compute\VirtualMachines.md");

            await generator.GenerateContentAsync(sourceFile);

            fileSystem
                .GetFile(@"c:\Output\Azure\Compute\VirtualMachines.html")
                .TextContents
                .Should()
                .Contain(@"<img src=""VM.png"" class=""img-fluid"" alt=""VMs"" />");
        }

        [Fact]
        public async Task ResolveLinkToAmbiguousImageInOtherFolder()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { @"c:\Content\Azure\Compute\VirtualMachines.md", new MockFileData("# This is about VMs ![VMs](VM.png)") },
                { @"c:\Content\Azure\Images\VM.png", new MockFileData(new byte[] { 0x1b, 0x2b, 0x3b }) },
            });
            var generator = CreateMarkdownToHtmlContentGenerator(fileSystem);
            var sourceFile = fileSystem.FileInfo.New(@"c:\Content\Azure\Compute\VirtualMachines.md");

            await generator.GenerateContentAsync(sourceFile);

            fileSystem
                .GetFile(@"c:\Output\Azure\Compute\VirtualMachines.html")
                .TextContents
                .Should()
                .Contain(@"<img src=""../Images/VM.png"" class=""img-fluid"" alt=""VMs"" />");
        }

        private static MarkdownToHtmlContentGenerator CreateMarkdownToHtmlContentGenerator(MockFileSystem fileSystem)
        {
            var contentDirectory = fileSystem.DirectoryInfo.New(@"c:\Content\");
            var outputDirectory = fileSystem.DirectoryInfo.New(@"c:\Output\");
            var generatorContext = new GeneratorContext(RazorEngineFactory.CreateRazorEngineService(), fileSystem,
                contentDirectory, outputDirectory);
            var generator = new MarkdownToHtmlContentGenerator(generatorContext);
            return generator;
        }
    }
}
