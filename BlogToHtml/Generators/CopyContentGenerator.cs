using System.IO.Abstractions;

namespace BlogToHtml.Generators
{
    using System.Threading.Tasks;

    internal class CopyContentGenerator : ContentGeneratorBase, IContentGenerator
    {
        public CopyContentGenerator(GeneratorContext generatorContext) : base(generatorContext)
        {
        }

        public override Task GenerateContentAsync(IFileInfo sourceFileInfo)
        {
            var outputFileInfo = GetOutputFileInfo(sourceFileInfo);
            EnsureOutputPathExists(outputFileInfo);

            sourceFileInfo.CopyTo(outputFileInfo.FullName, true);

            return Task.CompletedTask;
        }
    }
}
