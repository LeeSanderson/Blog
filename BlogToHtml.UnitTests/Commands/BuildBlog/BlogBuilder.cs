namespace BlogToHtml.UnitTests.Commands.BuildBlog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using BlogToHtml.Commands.BuildBlog;
    using BlogToHtml.Commands.BuildBlog.Models;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.Converters;
    using YamlDotNet.Serialization.NamingConventions;

    internal class BlogBuilder
    {
        private const string YamlDateFormat = "yyyy-MM-dd HH:mm:ss";
        private const string FrontMatterDelimiter = "---";

        private readonly DirectoryInfo inputDirectory;
        private readonly DirectoryInfo ouputDirectory;
        private readonly List<ArticleModel> articles = new List<ArticleModel>();
        private readonly ISerializer yamlSerialiser;

        public BlogBuilder()
        {
            inputDirectory = Path.Combine(".", "Input").ToDirectoryInfo(true);
            ouputDirectory = Path.Combine(".", "Output").ToDirectoryInfo(true);

            var builder = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithTypeConverter(new DateTimeConverter(DateTimeKind.Unspecified, null, YamlDateFormat));

            this.yamlSerialiser = builder.Build();
        }

        public BlogBuilder AddContent(
            string fileName, 
            string markdownContent, 
            string title = "Untitled Blogpost",
            string blogAbstract = "",
            string tags = "",
            DateTime? publicationDate = null)
        {
            var markdownContentFileName = Path.ChangeExtension(Path.Combine(inputDirectory.FullName, fileName), ".md");
            FileInfo markdownContentFile = new FileInfo(markdownContentFileName);
            articles.Add(
                new ArticleModel 
                { 
                    OutputFileInfo = markdownContentFile, 
                    Content = markdownContent,
                    Title = title,
                    Abstract = blogAbstract,
                    Tags = tags,
                    PublicationDate = publicationDate,
                });
            return this;
        }

        public async Task<BlogOutput> GenerateAsync()
        {
            await inputDirectory.CleanAsync();
            await ouputDirectory.CleanAsync();

            var output = new BlogOutput();
            foreach (var article in articles)
            {
                var frontMatter = FrontMatterDelimiter + Environment.NewLine + yamlSerialiser.Serialize(article) + Environment.NewLine + FrontMatterDelimiter;
                File.WriteAllText(article.OutputFileInfo.FullName, frontMatter + Environment.NewLine + Environment.NewLine + article.Content);
                output.InputFiles.Add(article.OutputFileInfo);
            }

            var buildBlogCommand =
                new BuildBlogCommandHandler(
                    new BuildBlogOptions
                    {
                        ContentDirectory = inputDirectory.FullName,
                        OutputDirectory = ouputDirectory.FullName,
                        Clean = true
                    });

            var result = await buildBlogCommand.RunAsync();
            if (result != 0)
            {
                throw new Exception($"Content generation failed with error code {result}");
            }

            ouputDirectory.Refresh();
            output.GeneratedFiles.AddRange(ouputDirectory.GetFiles());
            return output;
        }
    }

    internal sealed class BlogOutput : IDisposable
    {
        public List<FileInfo> InputFiles { get; } = new List<FileInfo>();
        public List<FileInfo> GeneratedFiles { get; } = new List<FileInfo>();

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
