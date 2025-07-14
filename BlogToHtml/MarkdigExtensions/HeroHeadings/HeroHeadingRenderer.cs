using System;
using System.Collections.Generic;
using System.IO;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace BlogToHtml.MarkdigExtensions.HeroHeadings;

internal class HeroHeadingRenderer : HtmlObjectRenderer<HeadingBlock>
{
    private readonly IMarkdownContext markdownContext;
    private readonly HeadingRenderer headingRenderer;

    public HeroHeadingRenderer(HeadingRenderer? headingRenderer, IMarkdownContext markdownContext)
    {
        this.markdownContext = markdownContext;
        this.headingRenderer = headingRenderer ?? new HeadingRenderer();
    }

    protected override void Write(HtmlRenderer renderer, HeadingBlock heading)
    {
        // Do what a regular heading rendered does
        headingRenderer.Write(renderer, heading);

        if (IsHeroHeading(heading))
        { 
            AddHeroImageToSource(renderer, GetInnerText(heading));
        }
    }

    private string GetInnerText(HeadingBlock heading)
    {
        Inline? inline = heading.Inline;
        var stringWriter = new StringWriter();
        HtmlRenderer renderer = new HtmlRenderer(stringWriter);
        while (inline != null)
        {
            renderer.Write(inline);
            inline = inline.NextSibling;
        }

        return stringWriter.ToString();
    }

    private bool IsHeroHeading(HeadingBlock heading)
    {
        var propertiesList = heading.GetAttributes().Properties;
        if (propertiesList != null)
        {
            var props = new Dictionary<string, string?>(propertiesList, StringComparer.OrdinalIgnoreCase);
            if (props.ContainsKey("data-hero-heading"))
            {
                return true;
            }
        }

        return false;
    }

    private void AddHeroImageToSource(HtmlRenderer renderer, string altText)
    {
        var currentRenderFile = this.markdownContext.CurrentSourceFile;
        if (currentRenderFile == null)
        {
            throw new Exception("Cannot render hero heading. CurrentSourceFile not set.");
        }

        var heroImageFileName = currentRenderFile.FileSystem.Path.ChangeExtension(currentRenderFile.Name, ".png");
        var imageAttributes = new HtmlAttributes();
        imageAttributes.AddProperty("src", heroImageFileName);
        imageAttributes.AddClass("hero-image");
        imageAttributes.AddProperty("alt", altText);
        renderer.Write("<img");
        renderer.WriteAttributes(imageAttributes);
        renderer.Writer.Write("/>");
        renderer.EnsureLine();
    }
}