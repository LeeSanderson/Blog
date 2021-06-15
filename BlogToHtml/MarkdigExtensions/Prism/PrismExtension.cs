namespace BlogToHtml.MarkdigExtensions.Prism
{
    using System;
    using Markdig;
    using Markdig.Renderers;
    using Markdig.Renderers.Html;

    internal class PrismExtension : IMarkdownExtension
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

            var exact = textRendererBase.ObjectRenderers.FindExact<CodeBlockRenderer>();
            if (exact != null)
            {
                textRendererBase.ObjectRenderers.Remove(exact);
            }

            textRendererBase.ObjectRenderers.AddIfNotAlready(new PrismCodeBlockRenderer(exact));
        }
    }
}