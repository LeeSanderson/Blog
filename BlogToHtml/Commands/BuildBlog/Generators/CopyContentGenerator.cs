namespace BlogToHtml.Commands.BuildBlog.Generators
{
    using System.IO;
    using System.Threading.Tasks;

    internal class CopyContentGenerator : ContentGeneratorBase, IContentGenerator
    {
        public CopyContentGenerator(GeneratorContext generatorContext): base(generatorContext)
        {
        }

        public override Task GenerateContentAsync(FileInfo sourceFileInfo)
        {
            var outputFileInfo = GetOutputFileInfo(sourceFileInfo);
            EnsureOutputPathExists(outputFileInfo);

            sourceFileInfo.CopyTo(outputFileInfo.FullName, true);

            return Task.CompletedTask;
        }
    }
}
