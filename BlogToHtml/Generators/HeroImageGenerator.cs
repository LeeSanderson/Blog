using System.Linq;
using PuppeteerSharp.BrowserData;

namespace BlogToHtml.Generators;

using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Models;
using PuppeteerSharp;

internal class HeroImageGenerator(GeneratorContext generatorContext) : GeneratorBase(generatorContext)
{
    private static readonly SemaphoreSlim BrowserFetcherSemaphore = new(1, 1);

    private readonly HeroImagesTemplate heroImagesTemplate = new(generatorContext.RazorEngineService);

    public async Task GenerateImageAsync(IFileInfo sourceFileInfo, HeroImageModel model)
    {
        var outputFileInfo = GetOutputFileInfo(sourceFileInfo, "png");
        EnsureOutputPathExists(outputFileInfo);
        var templateContext = new TemplateContext(GeneratorContext.OutputDirectory, outputFileInfo);

        var html = heroImagesTemplate.Generate(model, templateContext);
        if (model.OutputHtml)
        {
            var htmlOutputFileInfo = GetOutputFileInfo(sourceFileInfo, "png.html");
            await GeneratorContext.FileSystem.File.WriteAllTextAsync(htmlOutputFileInfo.FullName, html);
        }

        // var result = await ConvertHtmlToImage(html);
        var result = await ConvertHtmlToImageWithPuppeteer(html);
        await GeneratorContext.FileSystem.File.WriteAllBytesAsync(outputFileInfo.FullName, result);
    }

    private async Task<byte[]> ConvertHtmlToImageWithPuppeteer(string html)
    {
        await EnsureBrowserDownloaded();

        await using var browser = await Puppeteer.LaunchAsync(new()
        {
            Headless = true,
            // Headless = false,
            Args = new[] {"--no-sandbox", "--disable-setuid-sandbox"},
            DefaultViewport = new ViewPortOptions { Width = 800, Height = 100 }
        });

        var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);
        await Task.Delay(1000); // Wait for content to load?

        var options = new ScreenshotOptions { FullPage  = true, CaptureBeyondViewport = true };
        return await page.ScreenshotDataAsync(options);
    }

    private static async Task EnsureBrowserDownloaded()
    {
        // We need to fetch it
        await BrowserFetcherSemaphore.WaitAsync();
        try
        {
            var browserFetcher = new BrowserFetcher();
            var installedBrowser = 
                browserFetcher
                    .GetInstalledBrowsers()
                    .FirstOrDefault(b => b.Browser == browserFetcher.Browser && b.BuildId == Chrome.DefaultBuildId);

            if (installedBrowser == null)
            {
                await browserFetcher.DownloadAsync();
            }
        }
        finally
        {
            BrowserFetcherSemaphore.Release();
        }
    }

}