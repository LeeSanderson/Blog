namespace BlogToHtml
{
    using System.Threading.Tasks;
    using BlogToHtml.Commands.BuildBlog;
    using CommandLine;
    using Serilog;
    using Serilog.Events;

    class Program
    {
        static async Task Main(string[] args)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Is(LogEventLevel.Verbose)
                .WriteTo.Console();

            Log.Logger = loggerConfiguration.CreateLogger();

            await Parser.Default.ParseArguments<BuildBlogOptions>(args)
                .MapResult(
                    (BuildBlogOptions options) => new BuildBlogCommandHandler(options).RunAsync(),
                    errors => Task.FromResult(1));
        }
    }
}
