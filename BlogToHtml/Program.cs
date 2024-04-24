using System.IO.Abstractions;
using BlogToHtml.Commands.GenerateHeroImage;

namespace BlogToHtml
{
    using System.Threading.Tasks;
    using Commands.BuildBlog;
    using CommandLine;
    using Serilog;
    using Serilog.Events;

    static class Program
    {
        static async Task Main(string[] args)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Is(LogEventLevel.Verbose)
                .WriteTo.Console();

            Log.Logger = loggerConfiguration.CreateLogger();

            await Parser.Default.ParseArguments<
                    BuildBlogOptions,
                    GenerateHeroImageOptions
                >(args)
                .MapResult(
                    (BuildBlogOptions options) => new BuildBlogCommandHandler(new FileSystem(), options).RunAsync(),
                    (GenerateHeroImageOptions options) => new GenerateHeroImageCommandHandler(new FileSystem(), options).RunAsync(),
                    _ => Task.FromResult(1));
        }
    }
}
