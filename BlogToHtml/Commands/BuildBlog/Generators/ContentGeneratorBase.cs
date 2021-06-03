using System.IO;
using System.Threading.Tasks;

namespace BlogToHtml.Commands.BuildBlog.Generators
{
    internal abstract class ContentGeneratorBase : IContentGenerator
    {
        protected readonly GeneratorContext generatorContext;

        protected ContentGeneratorBase(GeneratorContext generatorContext)
        {
            this.generatorContext = generatorContext;
        }

        public abstract Task GenerateContentAsync(FileInfo sourceFileInfo);

        protected void EnsureOutputPathExists(FileInfo outputFileInfo)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputFileInfo.FullName));
        }

        protected FileInfo GetOutputFileInfo(FileInfo sourceFileInfo, string? newFileExtension = null)
        {
            var fileName = sourceFileInfo.FullName.Replace(generatorContext.ContentDirectory.FullName, generatorContext.OutputDirectory.FullName);

            if (newFileExtension != null)
            {
                fileName = Path.ChangeExtension(fileName, newFileExtension);
            }

            return new FileInfo(fileName);
        }
    }
}
