using System.IO.Abstractions;
using FluentAssertions;

namespace BlogToHtml.UnitTests.Commands.BuildBlog
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using VerifyXunit;
    using Xunit;

    [UsesVerify]
    public class BuildBlogCommandHandlerShould
    {
        [Fact]
        public async Task Generate_expected_HTML_from_markdown()
        {
            using var blogOutput = 
                await new BlogBuilder(new FileSystem())
                    .AddContent("Example.md", "# Heading 1", tags: new []{ "test", "with", "tags"})
                    .GenerateAsync(false);

            var generatedHtmlFile = blogOutput.GeneratedFiles.First(f => f.Name == "Example.html");
            await Verifier.VerifyFile(generatedHtmlFile.FullName);
        }

        [Fact]
        public async Task Generate_expected_hero_image()
        {
            using var blogOutput =
                await new BlogBuilder(new FileSystem())
                    .AddContent("Example.md", "# Heading 1")
                    .GenerateAsync(true);
            blogOutput.GeneratedFiles.First(f => f.Name == "Example.png").Should().NotBeNull();
        }


        [Fact]
        public async Task Generate_index_page_with_most_recent_10_blog_articles()
        {
            using var blogOutput = 
                await new BlogBuilder(new FileSystem())
                .AddContent("Example1.md", "# Heading 1", publicationDate: new DateTime(2022, 1, 1))
                .AddContent("Example2.md", "# Heading 2", publicationDate: new DateTime(2022, 1, 2))
                .AddContent("Example3.md", "# Heading 3", publicationDate: new DateTime(2022, 1, 3))
                .AddContent("Example4.md", "# Heading 4", publicationDate: new DateTime(2022, 1, 4))
                .AddContent("Example5.md", "# Heading 5", publicationDate: new DateTime(2022, 1, 5))
                .AddContent("Example6.md", "# Heading 6", publicationDate: new DateTime(2022, 1, 6))
                .AddContent("Example7.md", "# Heading 7", publicationDate: new DateTime(2022, 1, 7))
                .AddContent("Example8.md", "# Heading 8", publicationDate: new DateTime(2022, 1, 8))
                .AddContent("Example9.md", "# Heading 9", publicationDate: new DateTime(2022, 1, 9))
                .AddContent("Example10.md", "# Heading 10", publicationDate: new DateTime(2022, 1, 10))
                .AddContent("Example11.md", "# Heading 11", publicationDate: new DateTime(2022, 1, 11))
                .GenerateAsync(false);

            var generatedIndexHtmlFile = blogOutput.GeneratedFiles.First(f => f.Name == "index.html");
            await Verifier.VerifyFile(generatedIndexHtmlFile.FullName);
        }

        [Fact]
        public async Task Not_generate_pages_with_draft_status()
        {
            using var blogOutput =
                await new BlogBuilder(new FileSystem())
                    .AddContent("Example1.md", "# Heading 1", publicationDate: new DateTime(2022, 1, 1))
                    .AddContent("Example2.md", "# Heading 2", publicationDate: new DateTime(2022, 1, 2), publicationStatus: PublicationStatus.Draft)
                    .GenerateAsync(false);

            var generatedIndexHtmlFile = blogOutput.GeneratedFiles.First(f => f.Name == "index.html");
            await Verifier.VerifyFile(generatedIndexHtmlFile.FullName);
        }

        [Fact]
        public async Task Generate_all_page_all_blog_articles()
        {
            using var blogOutput =
                await new BlogBuilder(new FileSystem())
                .AddContent("Example1.md", "# Heading 1", publicationDate: new DateTime(2022, 1, 1))
                .AddContent("Example2.md", "# Heading 2", publicationDate: new DateTime(2022, 1, 2))
                .AddContent("Example3.md", "# Heading 3", publicationDate: new DateTime(2022, 1, 3))
                .AddContent("Example4.md", "# Heading 4", publicationDate: new DateTime(2022, 1, 4))
                .AddContent("Example5.md", "# Heading 5", publicationDate: new DateTime(2022, 1, 5))
                .AddContent("Example6.md", "# Heading 6", publicationDate: new DateTime(2022, 1, 6))
                .AddContent("Example7.md", "# Heading 7", publicationDate: new DateTime(2022, 1, 7))
                .AddContent("Example8.md", "# Heading 8", publicationDate: new DateTime(2022, 1, 8))
                .AddContent("Example9.md", "# Heading 9", publicationDate: new DateTime(2022, 1, 9))
                .AddContent("Example10.md", "# Heading 10", publicationDate: new DateTime(2022, 1, 10))
                .AddContent("Example11.md", "# Heading 11", publicationDate: new DateTime(2022, 1, 11))
                .GenerateAsync(false);

            var generatedIndexHtmlFile = blogOutput.GeneratedFiles.First(f => f.Name == "all.html");
            await Verifier.VerifyFile(generatedIndexHtmlFile.FullName);
        }
    }
}
