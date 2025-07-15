using System.Collections.Generic;
using BlogToHtml.Models;
using RazorEngine.Templating;

namespace BlogToHtml.Generators;

internal class AllTemplate(IRazorEngineService razorEngineService)
    : TemplateBase<List<SummaryModel>>(razorEngineService, "All");