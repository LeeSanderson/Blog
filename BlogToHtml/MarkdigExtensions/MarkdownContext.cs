using System.IO.Abstractions;

namespace BlogToHtml.MarkdigExtensions
{
    internal class MarkdownContext : IMarkdownContext
    {
        public static IMarkdownContext Current = new MarkdownContext();

        public IFileInfo? CurrentSourceFile { get; set; }
    }
}