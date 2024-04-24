namespace BlogToHtml.Commands.BuildBlog.Generators
{
    using Models;
    using RazorEngine.Templating;
    using System.Collections.Generic;

    internal class IndexTemplate : TemplateBase<List<SummaryModel>>
    {
        public IndexTemplate(IRazorEngineService razorEngineService) : base(razorEngineService, "Index")
        {
        }
    }
}
