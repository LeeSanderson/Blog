namespace BlogToHtml.Commands.BuildBlog.Generators
{
    using System.IO;
    using System.Threading.Tasks;
    using Models;
    using Markdig;
    using Markdig.Parsers;
    using Markdig.Syntax;
    using Markdig.Syntax.Inlines;
    using Markdig.Renderers.Html;
    using MarkdigExtensions.Prism;

    internal class MarkdownToHtmlContentGenerator : ContentGeneratorBase, IContentGenerator
    {
        private readonly ArticleTemplate articleTemplate;
        private readonly MarkdownPipeline pipeline;
        private readonly FrontMatterProcessor frontMatterProcessor;

        public MarkdownToHtmlContentGenerator(GeneratorContext generatorContext): base(generatorContext)
        {
            this.articleTemplate = new ArticleTemplate(generatorContext.RazorEngineService);
            this.pipeline = new MarkdownPipelineBuilder()
                .UseBootstrap()
                .UseYamlFrontMatter()
                .UseAdvancedExtensions()
                .UsePrism()
                .Build();

            this.frontMatterProcessor = new FrontMatterProcessor();
        }

        public override async Task GenerateContentAsync(FileInfo sourceFileInfo)
        {
            string markdownSource;
            using (var reader = sourceFileInfo.OpenText())
            {
                markdownSource = await reader.ReadToEndAsync();
            }

            var outputFileInfo = GetOutputFileInfo(sourceFileInfo, "html");
            EnsureOutputPathExists(outputFileInfo);

            var articleModel = ConvertMarkdownToModel(markdownSource);
            var templateContext = new TemplateContext(this.generatorContext.OutputDirectory, outputFileInfo);
            var htmlSource = articleTemplate.Generate(articleModel, templateContext);
            await using (var writer = outputFileInfo.CreateText())
            {
                await writer.WriteAsync(htmlSource);
            }
        }

        private ArticleModel ConvertMarkdownToModel(string markdownSource)
        {
            var document = MarkdownParser.Parse(markdownSource, this.pipeline);
            RewriteInternalLinks(document);
            return CreateArticleModel(document);
        }

        private ArticleModel CreateArticleModel(MarkdownDocument document)
        {
            var model = this.frontMatterProcessor.GetFrontMatter<ArticleModel>(document);
            model.Content = document.ToHtml(this.pipeline);
            return model;
        }

        private static void RewriteInternalLinks(MarkdownObject document)
        {
            foreach (var descendant in document.Descendants())
            {
                switch (descendant)
                {
                    case LinkInline link when link.Url != null && !link.Url.StartsWith("http"):
                        link.Url = link.Url.Replace(".md", ".html");
                        break;
                    case AutolinkInline:
                    case LinkInline:
                        descendant.GetAttributes().AddPropertyIfNotExist("target", "_blank");
                        break;
                }
            }
        }
    }
}
