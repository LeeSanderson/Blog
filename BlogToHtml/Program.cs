using BlogToHtml.Commands.BuildBlog;
using BlogToHtml.Commands.GenerateHeroImage;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.IO.Abstractions;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlogToHtml;

static class Program
{
    [STAThread]
    static async Task Main(string[] args)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(LogEventLevel.Verbose)
            .WriteTo.Console();

        Log.Logger = loggerConfiguration.CreateLogger();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddHttpClient();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();


        await Parser.Default.ParseArguments<
                BuildBlogOptions,
                GenerateHeroImageOptions
            >(args)
            .MapResult(
                (BuildBlogOptions options) => new BuildBlogCommandHandler(new FileSystem(), httpClientFactory, options).RunAsync(),
                (GenerateHeroImageOptions options) => new GenerateHeroImageCommandHandler(new FileSystem(), httpClientFactory, options).RunAsync(),
                _ => Task.FromResult(1));
    }
}