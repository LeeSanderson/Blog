using BlogToHtml.MarkdigExtensions.Prism;
using Markdig;

namespace BlogToHtml.Commands.BuildBlog.Generators
{
    public static class MarkdownPipelineFactory
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
