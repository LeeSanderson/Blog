namespace BlogToHtml.Generators
{
    using BlogToHtml.Models;
    using RazorEngine.Templating;
    using System.Collections.Generic;

    internal class AllTemplate : TemplateBase<List<SummaryModel>>
    {
        public AllTemplate(IRazorEngineService razorEngineService) : base(razorEngineService, "All")
        {
        }
    }
}
