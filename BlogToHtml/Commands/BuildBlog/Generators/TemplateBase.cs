using RazorEngine.Templating;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace BlogToHtml.Commands.BuildBlog.Generators
{
    internal abstract class TemplateBase<TModel>
    {
        private readonly IRazorEngineService razorEngineService;
        private readonly string templateKey;
        private bool compiled;

        protected TemplateBase(IRazorEngineService razorEngineService, string template)
        {
            this.templateKey = $"Templates.{template}";
            this.razorEngineService = razorEngineService;
        }
        
        public string Generate(TModel model, TemplateContext templateContext)
        {
            if (!compiled)
            {
                razorEngineService.Compile(templateKey, typeof(TModel));
                compiled = true;
            }

            var viewBag = new DynamicViewBag();
            viewBag.AddValue("TemplateContext", templateContext);
            return razorEngineService.Run(templateKey, typeof(TModel), model, viewBag);
        }

        public async Task GenerateAndSaveAsync(TModel model, TemplateContext templateContext, IFileInfo outputFileInfo)
        {
            var htmlSource = Generate(model, templateContext);
            await using var writer = outputFileInfo.CreateText();
            await writer.WriteAsync(htmlSource);
        }
    }
}