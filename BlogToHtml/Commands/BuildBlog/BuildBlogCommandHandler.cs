
namespace BlogToHtml.Commands.BuildBlog
{
    using BlogToHtml.Generators;
    using Models;
    using RazorEngine.Templating;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.IO.Abstractions;
    using BlogToHtml;

    public class BuildBlogCommandHandler
    {
        private readonly ILogger logger;
        private readonly IFileSystem fileSystem;
        private readonly BuildBlogOptions options;
        private readonly IRazorEngineService razorEngineService;
        private readonly Dictionary<string, IContentGenerator> contentGeneratorsByFileExtension = new();
        private readonly List<ISummaryContentGenerator> summaryContentGenerators = new();
        private readonly List<IEmbeddedContentGenerator> embeddedContentGenerators = new();

        public BuildBlogCommandHandler(IFileSystem fileSystem, BuildBlogOptions options)
        {
            logger = Log.ForContext<BuildBlogCommandHandler>();
            this.fileSystem = fileSystem;
            this.options = options;
            razorEngineService = RazorEngineFactory.CreateRazorEngineService();
        }

        public async Task<int> RunAsync()
        {
            try
            {
                var generatorContext = CreateGeneratorContext();
                RegisterEmbeddedContentGenerators(generatorContext);
                RegisterContentGenerators(generatorContext);
                RegisterSummaryContentGenerators(generatorContext);

                if (options.Clean)
                {
                    await generatorContext.OutputDirectory.CleanAsync();
                }

                await GenerateArticlesAsync(generatorContext);
                foreach (var summaryContentGenerator in summaryContentGenerators)
                {
                    await summaryContentGenerator.GenerateSummaryContentAsync();
                }

                foreach (var embeddedContentGenerator in embeddedContentGenerators)
                {
                    await embeddedContentGenerator.GenerateContentAsync();
                }

                return 0; // Success
            }
            catch(Exception e)
            {
                logger.Error(e, "Blog Builder Command Failed!");
                return 1; // Failure
            }
        }

        private void RegisterEmbeddedContentGenerators(GeneratorContext generatorContext)
        {
            embeddedContentGenerators.Clear();
            embeddedContentGenerators.Add(new EmbeddedContentGenerator(generatorContext, "site.css"));
        }

        private void RegisterSummaryContentGenerators(GeneratorContext generatorContext)
        {
            summaryContentGenerators.Clear();
            summaryContentGenerators.Add(new IndexPageGenerator(generatorContext));
        }

        private void RegisterContentGenerators(GeneratorContext generatorContext)
        {
            contentGeneratorsByFileExtension.Clear();
            var markdownToHtmlContentGenerator = new MarkdownToHtmlContentGenerator(generatorContext);
            contentGeneratorsByFileExtension[".md"] = markdownToHtmlContentGenerator;
            markdownToHtmlContentGenerator.ArticleGenerated += MarkdownToHtmlContentGeneratorArticleGenerated;

            var copyContentGenerator = new CopyContentGenerator(generatorContext);
            contentGeneratorsByFileExtension[".png"] = copyContentGenerator;
            contentGeneratorsByFileExtension[".css"] = copyContentGenerator;
        }

        private void MarkdownToHtmlContentGeneratorArticleGenerated(object? sender, ArticleModel model)
        {
            foreach (var summaryContentGenerator in summaryContentGenerators)
            {
                summaryContentGenerator.OnArticleGenerated(model);
            }
        }

        private GeneratorContext CreateGeneratorContext()
        {
            var contentDirectory = fileSystem.ToDirectoryInfo(options.ContentDirectory);
            var outputDirectory = fileSystem.ToDirectoryInfo(options.OutputDirectory, true);

            return new GeneratorContext(razorEngineService, fileSystem, contentDirectory, outputDirectory);
        }

        private async Task GenerateArticlesAsync(GeneratorContext generatorContext)
        {
            foreach (var f in generatorContext.ContentFiles)
            {
                // Execute generator base on file type
                // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
                var fileExtension = f.Extension?.ToLower() ?? string.Empty;
                if (contentGeneratorsByFileExtension.TryGetValue(fileExtension, out var contentGenerator))
                {
                    logger.Information("Generating article for {FileName}...", f.FullName);
                    await contentGenerator.GenerateContentAsync(f);
                }
                else
                {
                    logger.Warning(
                        "Skipping file {FileName} as no content generator registered for file extensions {FileExtension}",
                        f.FullName, fileExtension);
                }
            }
        }
    }
}
