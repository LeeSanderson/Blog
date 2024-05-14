using System.IO.Abstractions;

namespace BlogToHtml.UnitTests.Commands.BuildBlog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using BlogToHtml;
    using BlogToHtml.Commands.BuildBlog;
    using Models;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.Converters;
    using YamlDotNet.Serialization.NamingConventions;

    internal class BlogBuilder
    {
        private readonly IFileSystem fileSystem;
        private const string YamlDateFormat = "yyyy-MM-dd HH:mm:ss";
        private const string FrontMatterDelimiter = "---";

        private readonly IDirectoryInfo inputDirectory;
        private readonly IDirectoryInfo outputDirectory;
        private readonly List<ArticleModel> articles = new();
        private readonly ISerializer yamlSerializer;

        public BlogBuilder(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            inputDirectory = fileSystem.ToDirectoryInfo(Path.Combine(".", "Input"), true);
            outputDirectory = fileSystem.ToDirectoryInfo(Path.Combine(".", "Output"), true);

            var builder = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new DateTimeConverter(DateTimeKind.Unspecified, formats: YamlDateFormat));

            yamlSerializer = builder.Build();
        }

        public BlogBuilder AddContent(
            string fileName, 
            string markdownContent, 
            string title = "Untitled Blog post",
            string blogAbstract = "",
            string[]? tags = null,
            DateTime? publicationDate = null,
            PublicationStatus publicationStatus = PublicationStatus.Published)
        {
            var markdownContentFileName = Path.ChangeExtension(Path.Combine(inputDirectory.FullName, fileName), ".md");
            var markdownContentFile = fileSystem.FileInfo.New(markdownContentFileName);
            articles.Add(
                new ArticleModel 
                { 
                    OutputFileInfo = markdownContentFile, 
                    Content = markdownContent,
                    Title = title,
                    Abstract = blogAbstract,
                    Tags = tags,
                    PublicationDate = publicationDate,
                    PublicationStatus = publicationStatus
                });
            return this;
        }

        public async Task<BlogOutput> GenerateAsync(bool generateHeroImages)
        {
            await inputDirectory.CleanAsync();
            await outputDirectory.CleanAsync();

            var output = new BlogOutput();
            foreach (var article in articles)
            {
                var frontMatter = $"{FrontMatterDelimiter}{Environment.NewLine}{yamlSerializer.Serialize(article)}{Environment.NewLine}{FrontMatterDelimiter}";
                
                await fileSystem.File.WriteAllTextAsync(
                    article.OutputFileInfo!.FullName,
                    $"{frontMatter}{Environment.NewLine}{Environment.NewLine}{article.Content}");

                output.InputFiles.Add(article.OutputFileInfo);
            }

            var buildBlogCommand =
                new BuildBlogCommandHandler(
                    fileSystem,
                    new BuildBlogOptions
                    {
                        ContentDirectory = inputDirectory.FullName,
                        OutputDirectory = outputDirectory.FullName,
                        Clean = true,
                        GenerateHeroImages = generateHeroImages
                    });

            var result = await buildBlogCommand.RunAsync();
            if (result != 0)
            {
                throw new Exception($"Content generation failed with error code {result}");
            }

            outputDirectory.Refresh();
            output.GeneratedFiles.AddRange(outputDirectory.GetFiles());
            return output;
        }
    }

    internal sealed class BlogOutput : IDisposable
    {
        public List<IFileInfo> InputFiles { get; } = new();
        public List<IFileInfo> GeneratedFiles { get; } = new();

        public void Dispose()
        {
            foreach (var file in InputFiles)
            {
                file.Delete();
            }

            foreach (var file in GeneratedFiles)
            {
                file.Delete();
            }
        }
    }
}
