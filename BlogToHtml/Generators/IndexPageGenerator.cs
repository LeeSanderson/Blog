using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlogToHtml.Models;

namespace BlogToHtml.Generators;

internal class IndexPageGenerator(GeneratorContext generatorContext) : SummaryContentGeneratorBase(generatorContext)
{
    private readonly IndexTemplate indexTemplate = new(generatorContext.RazorEngineService);
    private readonly AllTemplate allTemplate = new(generatorContext.RazorEngineService);

    public override async Task GenerateSummaryContentAsync()
    {
        await GenerateSummaryPageAsync(indexTemplate, "index.html", Summaries.OrderByDescending(s => s.PublicationDate).Take(10).ToList());
        await GenerateSummaryPageAsync(allTemplate, "all.html", Summaries.OrderByDescending(s => s.PublicationDate).ToList());
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