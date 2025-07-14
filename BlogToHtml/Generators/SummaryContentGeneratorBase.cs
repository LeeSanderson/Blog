using System.Collections.Generic;
using System.Threading.Tasks;
using BlogToHtml.Models;

namespace BlogToHtml.Generators;

internal abstract class SummaryContentGeneratorBase(GeneratorContext generatorContext) : ISummaryContentGenerator
{
    protected readonly GeneratorContext GeneratorContext = generatorContext;
    protected readonly List<SummaryModel> Summaries = new();

    public abstract Task GenerateSummaryContentAsync();

    public void OnArticleGenerated(ArticleModel model)
    {
        Summaries.Add(model.Clone());
    }
}