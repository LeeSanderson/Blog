using System.IO.Abstractions;

namespace BlogToHtml.MarkdigExtensions
{
    public interface IMarkdownContext
    {
        IFileInfo? CurrentSourceFile { get; set; }
    }
}
