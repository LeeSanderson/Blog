namespace BlogToHtml
{
    using System.Threading.Tasks;
    using Commands.BuildBlog;
    using CommandLine;
    using Serilog;
    using Serilog.Events;
    using System.IO.Abstractions;
    using Commands.GenerateHeroImage;
    using System;

    static class Program
    {
        [STAThread]
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
