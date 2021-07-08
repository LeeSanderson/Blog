using System;
using System.Text;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace BlogToHtml.MarkdigExtensions.Prism
{
    internal class PrismCodeBlockRenderer : HtmlObjectRenderer<CodeBlock>
    {
        private readonly CodeBlockRenderer codeBlockRenderer;

        public PrismCodeBlockRenderer(CodeBlockRenderer? codeBlockRenderer) =>
            this.codeBlockRenderer = codeBlockRenderer ?? new CodeBlockRenderer();

        protected override void Write(HtmlRenderer renderer, CodeBlock node)
        {
            FencedCodeBlock? fencedCodeBlock = node as FencedCodeBlock;
            FencedCodeBlockParser? parser = node.Parser as FencedCodeBlockParser;
            if (fencedCodeBlock == null || parser == null)
            {
                codeBlockRenderer.Write(renderer, node);
                return;
            }

            string language = fencedCodeBlock.Info!.Replace(parser.InfoPrefix ?? string.Empty, string.Empty);
            if (string.IsNullOrWhiteSpace(language) || !PrismSupportedLanguages.IsSupportedLanguage(language))
            {
                codeBlockRenderer.Write(renderer, node);
                return;
            }

            var htmlAttributes = new HtmlAttributes();
            htmlAttributes.AddClass("language-" + language);
            string sourceCode = ExtractSourceCode((LeafBlock) node);
            renderer
                .Write("<pre>")
                    .Write("<code")
                        .WriteAttributes(htmlAttributes)
                        .Write(">")
                            .Write(sourceCode)
                    .Write("</code>")
                .Write("</pre>");
        }

        private static string ExtractSourceCode(LeafBlock node)
        {
            var stringBuilder = new StringBuilder();
            var lines = node.Lines.Lines;
            var length = lines.Length;
            for (var index = 0; index < length; ++index)
            {
                var slice = lines[index].Slice;
                if (slice.Text != null)
                {
                    string str = slice.Text.Substring(slice.Start, slice.Length);
                    if (index > 0)
                        stringBuilder.AppendLine();
                    stringBuilder.Append(str);
                }
            }
            return stringBuilder.ToString();
        }
    }
}