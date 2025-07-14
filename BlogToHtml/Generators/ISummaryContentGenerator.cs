using System.Threading.Tasks;
using BlogToHtml.Models;

namespace BlogToHtml.Generators;

internal interface ISummaryContentGenerator
{
    void OnArticleGenerated(ArticleModel model);

    Task GenerateSummaryContentAsync();
}