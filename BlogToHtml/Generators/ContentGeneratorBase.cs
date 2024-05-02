namespace BlogToHtml.Generators
{
    using System.IO.Abstractions;
    using System.Threading.Tasks;

    internal abstract class ContentGeneratorBase : GeneratorBase, IContentGenerator
    {
        protected ContentGeneratorBase(GeneratorContext generatorContext) : base(generatorContext)
        {
        }

        public abstract Task GenerateContentAsync(IFileInfo sourceFileInfo);
    }
}
