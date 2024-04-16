using System.IO.Abstractions;
using System.Threading.Tasks;

namespace BlogToHtml.Commands.BuildBlog.Generators
{
    public abstract class ContentGeneratorBase : GeneratorBase, IContentGenerator
    {
        protected ContentGeneratorBase(GeneratorContext generatorContext): base(generatorContext)
        {
        }

        public abstract Task GenerateContentAsync(IFileInfo sourceFileInfo);
    }
}
