using System.IO.Abstractions;
using System.Threading.Tasks;

namespace BlogToHtml.Generators;

internal abstract class ContentGeneratorBase(GeneratorContext generatorContext)
    : GeneratorBase(generatorContext), IContentGenerator
{
    public abstract Task GenerateContentAsync(IFileInfo sourceFileInfo);
}