using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace BlogToHtml.Commands.BuildBlog
{
    public static class RazorEngineFactory
    {
        public static IRazorEngineService CreateRazorEngineService()
        {
            var config = new TemplateServiceConfiguration
            {
                TemplateManager = new EmbeddedResourceTemplateManager(typeof(BuildBlogCommandHandler))
            };

            return RazorEngineService.Create(config);
        }
    }
}
