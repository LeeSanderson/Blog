namespace BlogToHtml.Generators
{
    using BlogToHtml.Models;
    using System.Threading.Tasks;

    internal abstract class SummaryContentGeneratorBase : ISummaryContentGenerator
    {
        protected readonly GeneratorContext GeneratorContext;

        protected SummaryContentGeneratorBase(GeneratorContext generatorContext)
        {
            GeneratorContext = generatorContext;
        }

        public abstract Task GenerateSummaryContentAsync();
        public abstract void OnArticleGenerated(ArticleModel model);
    }
}
