namespace BlogToHtml.Generators
{
    using System;
    using System.IO.Abstractions;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using Markdig;
    using Markdig.Parsers;
    using Markdig.Syntax;
    using Markdig.Syntax.Inlines;
    using Markdig.Renderers.Html;

    internal class MarkdownToHtmlContentGenerator : ContentGeneratorBase, IContentGenerator
    {
        private readonly ArticleTemplate articleTemplate;
        private readonly MarkdownPipeline pipeline;
        private readonly FrontMatterProcessor frontMatterProcessor;

        public event EventHandler<ArticleModel>? ArticleGenerated;

        public MarkdownToHtmlContentGenerator(GeneratorContext generatorContext) : base(generatorContext)
        {
            articleTemplate = new ArticleTemplate(generatorContext.RazorEngineService);
            pipeline = MarkdownPipelineFactory.GetStandardPipeline();
            frontMatterProcessor = new FrontMatterProcessor();
        }

        public override async Task GenerateContentAsync(IFileInfo sourceFileInfo)
        {
            string markdownSource;
            using (var reader = sourceFileInfo.OpenText())
            {
                markdownSource = await reader.ReadToEndAsync();

                // Replace non-breaking spaces added by Obsidian to support nested lists with regular spaces.
                markdownSource = markdownSource.Replace((char)0xa0, ' ');
            }

            var articleModel = ConvertMarkdownToModel(sourceFileInfo, markdownSource);
            if (articleModel.PublicationStatus == PublicationStatus.Published)
            {
                var outputFileInfo = GetOutputFileInfo(sourceFileInfo, "html");
                EnsureOutputPathExists(outputFileInfo);
                articleModel.OutputFileInfo = outputFileInfo;

                var templateContext = new TemplateContext(GeneratorContext.OutputDirectory, outputFileInfo);
                await articleTemplate.GenerateAndSaveAsync(articleModel, templateContext, outputFileInfo);

                ArticleGenerated?.Invoke(this, articleModel);
            }
        }

        private ArticleModel ConvertMarkdownToModel(IFileInfo sourceFileInfo, string markdownSource)
        {
            var document = MarkdownParser.Parse(markdownSource, pipeline);
            RewriteInternalLinks(sourceFileInfo, document);
            return CreateArticleModel(document);
        }

        private ArticleModel CreateArticleModel(MarkdownDocument document)
        {
            var model = frontMatterProcessor.GetFrontMatter<ArticleModel>(document);
            model.Content = document.ToHtml(pipeline);
            return model;
        }

        private void RewriteInternalLinks(IFileInfo sourceFileInfo, MarkdownObject document)
        {
            foreach (var descendant in document.Descendants())
            {
                switch (descendant)
                {
                    case LinkInline { Url: not null } link when !link.Url.StartsWith("http"):
                        link.Url = ResolveAmbiguousInternalLinks(sourceFileInfo, link.Url).Replace(".md", ".html");
                        break;
                    case AutolinkInline:
                    case LinkInline:
                        descendant.GetAttributes().AddPropertyIfNotExist("target", "_blank");
                        break;
                }
            }
        }

        private string ResolveAmbiguousInternalLinks(IFileInfo sourceFileInfo, string linkUrl)
        {
            // Resolve ambiguous Obsidian file links
            // Obsidian will create markdown links in the format `[Link](Link.md)` as long as Link.md is unique
            // If the name is not unique then the path will be included e.g. `[Link in folder 1](Output/Folder1/Link.md)`
            // and `[Link in Folder 2](Output/Folder2/Link.md)`
            if (linkUrl.Contains('/'))
            {
                return linkUrl; // Assume name already resolved
            }

            // Find file
            var linkedFile = GeneratorContext.ContentFiles.FirstOrDefault(x => x.Name == linkUrl);
            if (linkedFile == null)
            {
                return linkUrl;
            }

            return sourceFileInfo.GetRelativePathTo(linkedFile);
        }
    }
}
