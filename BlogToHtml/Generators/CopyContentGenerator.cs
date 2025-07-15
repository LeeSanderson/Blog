using System.IO.Abstractions;
using System.Threading.Tasks;

namespace BlogToHtml.Generators;

internal class CopyContentGenerator(GeneratorContext generatorContext)
    : ContentGeneratorBase(generatorContext), IContentGenerator
{
    public override Task GenerateContentAsync(IFileInfo sourceFileInfo)
    {
        var outputFileInfo = GetOutputFileInfo(sourceFileInfo);
        EnsureOutputPathExists(outputFileInfo);

        sourceFileInfo.CopyTo(outputFileInfo.FullName, true);

        return Task.CompletedTask;
    }
}