using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using BlogToHtml.Generators;
using FluentAssertions;
using Xunit;

namespace BlogToHtml.UnitTests.Generators
{
    public class MarkdownToHtmlContentGeneratorShould
    {
        [Fact]
        public async Task GenerateExpectedHtml()
        {
            var outputFile = await GenerateFile("## This is a H2");
            outputFile
                .TextContents
                .Should()
                .Contain(@"<h2 id=""this-is-a-h2"">This is a H2</h2>");
        }

        [Fact]
        public async Task MarkFirstLevel1HeadingAsHeroHeading()
        {
            var outputFile = await GenerateFile("# This is a H1" + Environment.NewLine + "# This is another H1");
            outputFile
                .TextContents
                .Should()
                .Contain(@"<h1 id=""this-is-a-h1"" class=""sr-only"" data-hero-heading=""true"">This is a H1</h1>")
                .And.Contain(@"<h1 id=""this-is-another-h1"">This is another H1</h1>");
        }

        [Fact]
        public async Task GeneratesBulletList()
        {
            var outputFile = await GenerateFile(" - one");
            outputFile
                .TextContents
                .RemoveAllWhiteSpace()
                .Should()
                .Contain(@"<ul><li>one</li></ul>");
        }

        [Fact]
        public async Task GeneratesNestedBulletList()
        {
            var outputFile = await GenerateFile(@" 
- one
    - two");
            outputFile
                .TextContents
                .RemoveAllWhiteSpace()
                .Should()
                .Contain(@"<ul><li>one<ul><li>two</li></ul></li></ul>");
        }

        [Fact]
        public async Task GeneratesNestedBulletListWhenInputContainsNonBreakingSpaces()
        {
            var outputFile = await GenerateFile(@" 
- one
" + (char)0xa0 + (char)0xa0 + " - two");
            outputFile
                .TextContents
                .RemoveAllWhiteSpace()
                .Should()
                .Contain(@"<ul><li>one<ul><li>two</li></ul></li></ul>");
        }


        [Fact]
        public async Task GenerateLinkToOtherArticle()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { @"c:\Content\Azure\Compute\VirtualMachines.md", CreateArticleFrom("# This is about VMs") },
                { @"c:\Content\Azure\Storage\StorageAccounts.md", CreateArticleFrom("# This is about storage of [VMs](../Compute/VirtualMachines.md)") },
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
                { @"c:\Content\Azure\Compute\VirtualMachines.md", CreateArticleFrom("# This is about VMs") },
                { @"c:\Content\Azure\Compute\VMTypes.md", CreateArticleFrom("# This is about types of [VMs](VirtualMachines.md)") },
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
                { @"c:\Content\Azure\Compute\VirtualMachines.md", CreateArticleFrom("# This is about VMs") },
                { @"c:\Content\Azure\Storage\StorageAccounts.md", CreateArticleFrom("# This is about storage of [VMs](VirtualMachines.md)") },
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
                { @"c:\Content\Azure\Compute\VirtualMachines.md", CreateArticleFrom("# This is about VMs ![VMs](VM.png)") },
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
                { @"c:\Content\Azure\Compute\VirtualMachines.md", CreateArticleFrom("# This is about VMs ![VMs](VM.png)") },
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
            generator.GenerateHeroImages = false;
            return generator;
        }

        private static async Task<MockFileData> GenerateFile(string content)
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { @"c:\Content\Test1.md", CreateArticleFrom(content) }
            });
            var generator = CreateMarkdownToHtmlContentGenerator(fileSystem);
            var sourceFile = fileSystem.FileInfo.New(@"c:\Content\Test1.md");

            await generator.GenerateContentAsync(sourceFile);

            return fileSystem.GetFile(@"c:\Output\Test1.html");
        }

        private static MockFileData CreateArticleFrom(string markdown)
        {
            const string frontMatter = "---\nstatus: Published\n---\n\n";
            return new MockFileData(frontMatter + markdown);
        }
    }
}
