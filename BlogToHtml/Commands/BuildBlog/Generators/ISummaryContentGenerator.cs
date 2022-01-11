namespace BlogToHtml.Commands.BuildBlog.Generators
{
    using BlogToHtml.Commands.BuildBlog.Models;
    using System.Threading.Tasks;

    public interface ISummaryContentGenerator
    {
        void OnArticleGenerated(ArticleModel model);

        Task GenerateSummaryContentAsync();
    }
}
