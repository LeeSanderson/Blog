using System.IO.Abstractions;

namespace BlogToHtml.Commands.BuildBlog.Generators
{
    using System.Threading.Tasks;

    public interface IContentGenerator
    {
        Task GenerateContentAsync(IFileInfo sourceFileInfo);
    }
}
