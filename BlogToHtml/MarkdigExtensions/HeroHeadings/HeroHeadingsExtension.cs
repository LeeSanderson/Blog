using Markdig;
using Markdig.Renderers;
using System;
using Markdig.Renderers.Html;

namespace BlogToHtml.MarkdigExtensions.HeroHeadings;

internal class HeroHeadingsExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer == null)
            throw new ArgumentNullException(nameof(renderer));
        if (renderer is not TextRendererBase<HtmlRenderer> textRendererBase)
            return;

        var standardHeadingRenderer = textRendererBase.ObjectRenderers.FindExact<HeadingRenderer>();
        if (standardHeadingRenderer != null)
        {
            textRendererBase.ObjectRenderers.Remove(standardHeadingRenderer);
        }

        textRendererBase.ObjectRenderers.AddIfNotAlready(new HeroHeadingRenderer(standardHeadingRenderer, MarkdownContext.Current));
    }
}