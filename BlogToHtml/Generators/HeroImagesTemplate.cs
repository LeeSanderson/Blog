namespace BlogToHtml.Generators;

using Models;
using RazorEngine.Templating;

internal class HeroImagesTemplate(IRazorEngineService razorEngineService)
    : TemplateBase<HeroImageModel>(razorEngineService, "HeroImage");