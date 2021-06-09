namespace BlogToHtml.Commands.BuildBlog.Generators
{
    using Models;
    using RazorEngine.Templating;

    internal class ArticleTemplate : TemplateBase<ArticleModel>
    {
        public ArticleTemplate(IRazorEngineService razorEngineService) : base(razorEngineService, "Article")
        {
        }
    }
}
