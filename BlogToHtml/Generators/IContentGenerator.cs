using System.IO.Abstractions;

namespace BlogToHtml.Generators
{
    using System.Threading.Tasks;

    public interface IContentGenerator
    {
        Task GenerateContentAsync(IFileInfo sourceFileInfo);
    }
}
