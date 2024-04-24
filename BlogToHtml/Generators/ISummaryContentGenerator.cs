namespace BlogToHtml.Generators
{
    using BlogToHtml.Models;
    using System.Threading.Tasks;

    public interface ISummaryContentGenerator
    {
        void OnArticleGenerated(ArticleModel model);

        Task GenerateSummaryContentAsync();
    }
}
