namespace BlogToHtml.Generators
{
    using BlogToHtml.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal abstract class SummaryContentGeneratorBase : ISummaryContentGenerator
    {
        protected readonly GeneratorContext GeneratorContext;
        protected readonly List<SummaryModel> Summaries = new();

        protected SummaryContentGeneratorBase(GeneratorContext generatorContext)
        {
            GeneratorContext = generatorContext;
        }

        public abstract Task GenerateSummaryContentAsync();

        public void OnArticleGenerated(ArticleModel model)
        {
            Summaries.Add(model.Clone());
        }
    }
}
