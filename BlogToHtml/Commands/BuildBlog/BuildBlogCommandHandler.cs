namespace BlogToHtml.Commands.BuildBlog
{
    using BlogToHtml.Commands.BuildBlog.Generators;
    using BlogToHtml.Commands.BuildBlog.Models;
    using RazorEngine.Configuration;
    using RazorEngine.Templating;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class BuildBlogCommandHandler
    {
        private readonly ILogger logger;
        private readonly BuildBlogOptions options;
        private readonly IRazorEngineService razorEngineService;
        private readonly Dictionary<string, IContentGenerator> contentGeneratorsByFileExtension = new Dictionary<string, IContentGenerator>();
        private readonly List<ISummaryContentGenerator> summaryContentGenerators = new List<ISummaryContentGenerator>();

        public BuildBlogCommandHandler(BuildBlogOptions options)
        {
            logger = Log.ForContext<BuildBlogCommandHandler>();
            this.options = options;

            var config = new TemplateServiceConfiguration
            {
                TemplateManager = new EmbeddedResourceTemplateManager(typeof(BuildBlogCommandHandler))
            };
            this.razorEngineService = RazorEngineService.Create(config);
        }

        public async Task<int> RunAsync()
        {
            try
            {
                var generatorContext = CreateGeneratorContext();
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
                
                return 0; // Success
            }
            catch(Exception e)
            {
                logger.Error(e, "Blog Builder Command Failed!");
                return 1; // Failure
            }
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
            var contentDirectory = options.ContentDirectory.ToDirectoryInfo();
            var outputDirectory = options.OutputDirectory.ToDirectoryInfo(true);

            return new GeneratorContext(razorEngineService, contentDirectory, outputDirectory);
        }

        private async Task GenerateArticlesAsync(GeneratorContext generatorContext)
        {
            await generatorContext.ContentDirectory.RecurseAsync(async f =>
            {
                // Execute generator base on file type
                var fileExtention = f.Extension?.ToLower() ?? string.Empty;
                if (contentGeneratorsByFileExtension.TryGetValue(fileExtention, out var contentGenerator))
                {
                    logger.Information("Generating article for {FileName}...", f.FullName);
                    await contentGenerator.GenerateContentAsync(f);
                }
                else
                {
                    logger.Warning("Skipping file {FileName} as no content generator registered for file extensions {FileExtension}", f.FullName, fileExtention);
                }
            });
        }
    }
}
