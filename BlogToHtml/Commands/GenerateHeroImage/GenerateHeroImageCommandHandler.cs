using System.Threading.Tasks;

namespace BlogToHtml.Commands.GenerateHeroImage
{
    using RazorEngine.Templating;
    using Serilog;
    using System.IO.Abstractions;

    public class GenerateHeroImageCommandHandler
    {
        private readonly ILogger logger;
        private readonly IFileSystem fileSystem;
        private readonly GenerateHeroImageOptions options;
        private readonly IRazorEngineService razorEngineService;

        public GenerateHeroImageCommandHandler(IFileSystem fileSystem, GenerateHeroImageOptions options)
        {
            logger = Log.ForContext<GenerateHeroImageCommandHandler>();
            this.fileSystem = fileSystem;
            this.options = options;
            razorEngineService = RazorEngineFactory.CreateRazorEngineService();
        }

        public Task<int> RunAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
