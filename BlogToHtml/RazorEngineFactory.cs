using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace BlogToHtml
{
    public static class RazorEngineFactory
    {
        public static IRazorEngineService CreateRazorEngineService()
        {
            var config = new TemplateServiceConfiguration
            {
                TemplateManager = new EmbeddedResourceTemplateManager(typeof(RazorEngineFactory))
            };

            return RazorEngineService.Create(config);
        }
    }
}
