namespace BlogToHtml.Generators
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
