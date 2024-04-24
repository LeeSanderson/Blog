namespace BlogToHtml.Generators
{
    using BlogToHtml.Models;
    using System.Threading.Tasks;

    internal abstract class SummaryContentGeneratorBase : ISummaryContentGenerator
    {
        protected readonly GeneratorContext generatorContext;

        protected SummaryContentGeneratorBase(GeneratorContext generatorContext)
        {
            this.generatorContext = generatorContext;
        }

        public abstract Task GenerateSummaryContentAsync();
        public abstract void OnArticleGenerated(ArticleModel model);
    }
}
