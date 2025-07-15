using System.IO;
using System.IO.Abstractions;

namespace BlogToHtml.Generators;

internal class GeneratorBase
{
    protected readonly GeneratorContext GeneratorContext;

    protected GeneratorBase(GeneratorContext generatorContext)
    {
        GeneratorContext = generatorContext;
    }

    protected void EnsureOutputPathExists(IFileInfo outputFileInfo)
    {
        GeneratorContext.FileSystem.Directory.CreateDirectory(Path.GetDirectoryName(outputFileInfo.FullName)!);
    }

    protected IFileInfo GetOutputFileInfo(IFileInfo sourceFileInfo, string? newFileExtension = null) =>
        GeneratorContext.FileSystem.FileInfo.New(GetOutputFileName(sourceFileInfo.FullName, newFileExtension));

    protected string GetOutputFileName(string fullSourceFileName, string? newFileExtension = null)
    {
        var fileName = fullSourceFileName.Replace(GeneratorContext.ContentDirectory.FullName, GeneratorContext.OutputDirectory.FullName);

        if (newFileExtension != null)
        {
            fileName = Path.ChangeExtension(fileName, newFileExtension);
        }

        return fileName;
    }
}