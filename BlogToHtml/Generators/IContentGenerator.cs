using System.IO.Abstractions;

namespace BlogToHtml.Generators
{
    using System.Threading.Tasks;

    internal interface IContentGenerator
    {
        Task GenerateContentAsync(IFileInfo sourceFileInfo);
    }
}
