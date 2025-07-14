using BlogToHtml.MarkdigExtensions.HeroHeadings;
using BlogToHtml.MarkdigExtensions.Prism;
using Markdig;

namespace BlogToHtml.MarkdigExtensions;


internal static class MarkdownExtensions
{
    public static MarkdownPipelineBuilder UsePrism(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.AddIfNotAlready<PrismExtension>();
        return pipeline;
    }

    public static MarkdownPipelineBuilder UseHeroHeadings(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.AddIfNotAlready<HeroHeadingsExtension>();
        return pipeline;
    }
}