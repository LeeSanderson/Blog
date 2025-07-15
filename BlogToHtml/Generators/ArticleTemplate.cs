using BlogToHtml.Models;
using RazorEngine.Templating;

namespace BlogToHtml.Generators;

internal class ArticleTemplate(IRazorEngineService razorEngineService)
    : TemplateBase<ArticleModel>(razorEngineService, "Article");