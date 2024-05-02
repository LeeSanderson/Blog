namespace BlogToHtml.Generators
{
    using Models;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    internal class IndexPageGenerator : SummaryContentGeneratorBase
    {
        private readonly IndexTemplate indexTemplate;
        private readonly AllTemplate allTemplate;
        private readonly List<SummaryModel> summaries = new();

        public IndexPageGenerator(GeneratorContext generatorContext) : base(generatorContext)
        {
            indexTemplate = new IndexTemplate(generatorContext.RazorEngineService);
            allTemplate = new AllTemplate(generatorContext.RazorEngineService);
        }

        public override async Task GenerateSummaryContentAsync()
        {
            await GenerateSummaryPageAsync(indexTemplate, "index.html", summaries.OrderByDescending(s => s.PublicationDate).Take(10).ToList());
            await GenerateSummaryPageAsync(allTemplate, "all.html", summaries.OrderByDescending(s => s.PublicationDate).ToList());
        }

        public override void OnArticleGenerated(ArticleModel model)
        {
            summaries.Add(model.Clone());
        }

        private async Task GenerateSummaryPageAsync(
            TemplateBase<List<SummaryModel>> template,
            string outputFileName,
            List<SummaryModel> pageSummaries)
        {
            var pageFileInfo = GeneratorContext.FileSystem.FileInfo.New(Path.Combine(GeneratorContext.OutputDirectory.FullName, outputFileName));
            var templateContext = new TemplateContext(GeneratorContext.OutputDirectory, pageFileInfo);
            await template.GenerateAndSaveAsync(
                pageSummaries,
                templateContext,
                pageFileInfo);
        }
    }
}
