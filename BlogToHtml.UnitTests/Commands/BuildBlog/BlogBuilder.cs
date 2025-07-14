using BlogToHtml.Commands.BuildBlog;
using BlogToHtml.Models;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Net.Http;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.NamingConventions;
using RichardSzalay.MockHttp;

namespace BlogToHtml.UnitTests.Commands.BuildBlog;

internal class BlogBuilder
{
    private readonly IFileSystem fileSystem;
    private readonly MockHttpMessageHandler mockHttpMessageHandler = new();
    private const string YamlDateFormat = "yyyy-MM-dd HH:mm:ss";
    private const string FrontMatterDelimiter = "---";

    private readonly IDirectoryInfo inputDirectory;
    private readonly IDirectoryInfo outputDirectory;
    private readonly List<ArticleModel> articles = [];
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
        PublicationStatus publicationStatus = PublicationStatus.Published,
        string? notebookUrl = null)
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
                PublicationStatus = publicationStatus,
                NotebookUrl = notebookUrl
            });
        return this;
    }

    public BlogBuilder AddExternalNotebookContent(
        string notebookUrl,
        string notebookContent)
    {
        mockHttpMessageHandler.When(notebookUrl)
            .Respond("application/vnd.jupyter", notebookContent);

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

        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient(mockHttpMessageHandler));

        var buildBlogCommand =
            new BuildBlogCommandHandler(
                fileSystem,
                httpClientFactory,
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