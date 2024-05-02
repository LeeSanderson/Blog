using BlogToHtml.MarkdigExtensions.Prism;
using Markdig;

namespace BlogToHtml.Generators
{
    internal static class MarkdownPipelineFactory
    {
        public static MarkdownPipeline GetStandardPipeline() =>
            new MarkdownPipelineBuilder()
                .UseBootstrap()
                .UseYamlFrontMatter()
                .UseAdvancedExtensions()
                .UsePrism()
                .Build();

    }
}
