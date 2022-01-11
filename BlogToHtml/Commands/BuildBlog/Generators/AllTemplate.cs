namespace BlogToHtml.Commands.BuildBlog.Generators
{
    using BlogToHtml.Commands.BuildBlog.Models;
    using RazorEngine.Templating;
    using System.Collections.Generic;

    internal class AllTemplate : TemplateBase<List<SummaryModel>>
    {
        public AllTemplate(IRazorEngineService razorEngineService) : base(razorEngineService, "All")
        {
        }
    }
}
