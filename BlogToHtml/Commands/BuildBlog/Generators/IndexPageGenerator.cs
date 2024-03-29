﻿namespace BlogToHtml.Commands.BuildBlog.Generators
{
    using BlogToHtml.Commands.BuildBlog.Models;
    using System;
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
            this.indexTemplate = new IndexTemplate(generatorContext.RazorEngineService);
            this.allTemplate = new AllTemplate(generatorContext.RazorEngineService);
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

        public async Task GenerateSummaryPageAsync(TemplateBase<List<SummaryModel>> template, string outputFileName, List<SummaryModel> pageSummaries)
        {
            var pageFileInfo = new FileInfo(Path.Combine(generatorContext.OutputDirectory.FullName, outputFileName));
            var templateContext = new TemplateContext(generatorContext.OutputDirectory, pageFileInfo);
            await template.GenerateAndSaveAsync(
                pageSummaries,
                templateContext,
                pageFileInfo);
        }
    }
}
