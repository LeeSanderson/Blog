using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace BlogToHtml.Commands.BuildBlog.Generators
{
    internal abstract class ContentGeneratorBase : IContentGenerator
    {
        protected readonly GeneratorContext GeneratorContext;

        protected ContentGeneratorBase(GeneratorContext generatorContext)
        {
            GeneratorContext = generatorContext;
        }

        public abstract Task GenerateContentAsync(IFileInfo sourceFileInfo);

        protected void EnsureOutputPathExists(IFileInfo outputFileInfo)
        {
            GeneratorContext.FileSystem.Directory.CreateDirectory(Path.GetDirectoryName(outputFileInfo.FullName)!);
        }

        protected IFileInfo GetOutputFileInfo(IFileInfo sourceFileInfo, string? newFileExtension = null)
        {
            var fileName = sourceFileInfo.FullName.Replace(GeneratorContext.ContentDirectory.FullName, GeneratorContext.OutputDirectory.FullName);

            if (newFileExtension != null)
            {
                fileName = Path.ChangeExtension(fileName, newFileExtension);
            }

            return GeneratorContext.FileSystem.FileInfo.New(fileName);
        }
    }
}
