namespace BlogToHtml.Commands.BuildBlog.Generators
{
    using BlogToHtml.Commands.BuildBlog.Models;
    using RazorEngine.Templating;
    using System.Collections.Generic;

    internal class IndexTemplate : TemplateBase<List<SummaryModel>>
    {
        public IndexTemplate(IRazorEngineService razorEngineService) : base(razorEngineService, "Index")
        {
        }
    }
}
