namespace BlogToHtml.Generators;

using System.Collections.Generic;
using Models;
using RazorEngine.Templating;

internal class IndexTemplate(IRazorEngineService razorEngineService)
    : TemplateBase<List<SummaryModel>>(razorEngineService, "Index");