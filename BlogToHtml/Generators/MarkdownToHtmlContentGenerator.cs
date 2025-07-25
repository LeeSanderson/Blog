﻿using BlogToHtml.MarkdigExtensions;
using BlogToHtml.Models;
using Markdig;
using Markdig.Parsers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using BlogToHtml.Notebooks;

namespace BlogToHtml.Generators;

internal class MarkdownToHtmlContentGenerator(GeneratorContext generatorContext)
    : ContentGeneratorBase(generatorContext), IContentGenerator
{
    private readonly ArticleTemplate articleTemplate = new(generatorContext.RazorEngineService);
    private readonly MarkdownPipeline pipeline = MarkdownPipelineFactory.GetStandardPipeline();
    private readonly FrontMatterProcessor frontMatterProcessor = new();
    private readonly HeroImageGenerator heroImageGenerator = new(generatorContext);

    public event EventHandler<ArticleModel>? ArticleGenerated;
    public bool GenerateHeroImages { get; set; } = true;

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
            if (!string.IsNullOrEmpty(articleModel.NotebookUrl))
            {
                articleModel = await ConvertNotebookToModel(articleModel.NotebookUrl, sourceFileInfo, markdownSource);
            }

            var outputFileInfo = GetOutputFileInfo(sourceFileInfo, "html");
            EnsureOutputPathExists(outputFileInfo);
            articleModel.OutputFileInfo = outputFileInfo;

            var templateContext = new TemplateContext(GeneratorContext.OutputDirectory, outputFileInfo);
            await articleTemplate.GenerateAndSaveAsync(articleModel, templateContext, outputFileInfo);

            if (GenerateHeroImages)
            {
                var heroImageModel = new HeroImageModel {Title = articleModel.Title, Tags = articleModel.Tags};
                await heroImageGenerator.GenerateImageAsync(sourceFileInfo, heroImageModel);
            }

            ArticleGenerated?.Invoke(this, articleModel);
        }
    }

    private async Task<ArticleModel> ConvertNotebookToModel(string notebookUrl, IFileInfo sourceFileInfo, string fileMarkdownSource)
    {
        var httpClient = GeneratorContext.HttpClientFactory.CreateClient(string.Empty);
        var response = await httpClient.GetAsync(notebookUrl);
        response.EnsureSuccessStatusCode();
        var notebookSource = await response.Content.ReadAsStringAsync();
        var notebookConverter = new NotebookConverter();
        var notebook = notebookConverter.Convert(notebookSource);

        var (frontMatter, additionalMarkdown) = frontMatterProcessor.SplitFrontMatter(fileMarkdownSource);
        var combinedMarkdown =
            frontMatter
            + Environment.NewLine
            + notebook.Markdown
            + Environment.NewLine
            + additionalMarkdown;

        return ConvertMarkdownToModel(sourceFileInfo, combinedMarkdown);
    }

    private ArticleModel ConvertMarkdownToModel(IFileInfo sourceFileInfo, string markdownSource)
    {
        MarkdownContext.Current.CurrentSourceFile = sourceFileInfo;
        var document = MarkdownParser.Parse(markdownSource, pipeline);
        MarkFirstHeadingAsHeroImage(document);
        RewriteInternalLinks(sourceFileInfo, document);
        return CreateArticleModel(document);
    }

    private ArticleModel CreateArticleModel(MarkdownDocument document)
    {
        var model = frontMatterProcessor.GetFrontMatter<ArticleModel>(document);
        model.Content = document.ToHtml(pipeline);
        return model;
    }

    private void MarkFirstHeadingAsHeroImage(MarkdownDocument document)
    {
        foreach (var descendant in document.Descendants())
        {
            switch (descendant)
            {
                case HeadingBlock { Level: 1 } heading:
                    heading.GetAttributes().AddPropertyIfNotExist("class", "sr-only");
                    heading.GetAttributes().AddPropertyIfNotExist("data-hero-heading", "true");
                    return; // Only apply change to first heading
            }
        }
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