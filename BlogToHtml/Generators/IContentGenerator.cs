using System.IO.Abstractions;
using System.Threading.Tasks;

namespace BlogToHtml.Generators;

internal interface IContentGenerator
{
    Task GenerateContentAsync(IFileInfo sourceFileInfo);
}