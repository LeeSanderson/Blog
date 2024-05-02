namespace BlogToHtml.Generators
{
    using BlogToHtml.Models;
    using System.Threading.Tasks;

    internal interface ISummaryContentGenerator
    {
        void OnArticleGenerated(ArticleModel model);

        Task GenerateSummaryContentAsync();
    }
}
