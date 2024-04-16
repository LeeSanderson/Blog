using System.IO.Abstractions;

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

            await Parser.Default.ParseArguments<BuildBlogOptions>(args)
                .MapResult(
                    options => new BuildBlogCommandHandler(new FileSystem(), options).RunAsync(),
                    errors => Task.FromResult(1));
        }
    }
}
