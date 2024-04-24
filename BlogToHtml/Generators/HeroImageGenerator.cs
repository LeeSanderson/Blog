﻿namespace BlogToHtml.Generators
{
    using System.IO.Abstractions;
    using System.Threading.Tasks;
    using WkHtmlConverter;
    using Models;

    public class HeroImageGenerator : GeneratorBase
    {
        private readonly HeroImagesTemplate heroImagesTemplate;
        private readonly HtmlToImageConverter htmlToImageConverter;

        public HeroImageGenerator(GeneratorContext generatorContext) : base(generatorContext)
        {
            heroImagesTemplate = new HeroImagesTemplate(generatorContext.RazorEngineService);
            htmlToImageConverter = new HtmlToImageConverter();
        }

        public async Task GenerateImageAsync(IFileInfo sourceFileInfo, HeroImageModel model)
        {
            var outputFileInfo = GetOutputFileInfo(sourceFileInfo, "png");
            EnsureOutputPathExists(outputFileInfo);
            var templateContext = new TemplateContext(GeneratorContext.OutputDirectory, outputFileInfo);

            var imageConversionSettings = new ImageConversionSettings { Format = ImageOutputFormat.Png };
            var html = heroImagesTemplate.Generate(model, templateContext);
            var result = await htmlToImageConverter.ConvertAsync(imageConversionSettings, html);

            await GeneratorContext.FileSystem.File.WriteAllBytesAsync(outputFileInfo.FullName, result);
        }
    }
}