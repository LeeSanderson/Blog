namespace BlogToHtml.MarkdigExtensions.Prism
{
    using Markdig;

    internal static class PrismMarkdownExtensions
    {
        public static MarkdownPipelineBuilder UsePrism(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.AddIfNotAlready<PrismExtension>();
            return pipeline;
        }

    }
}
