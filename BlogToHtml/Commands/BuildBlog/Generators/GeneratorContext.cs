namespace BlogToHtml.Commands.BuildBlog.Generators
{
    using RazorEngine.Templating;
    using System.IO;
    internal class GeneratorContext
    {
        public IRazorEngineService RazorEngineService { get; }
        public DirectoryInfo ContentDirectory { get; }
        public DirectoryInfo OutputDirectory { get; }

        public GeneratorContext(IRazorEngineService razorEngineService, DirectoryInfo contentDirectory, DirectoryInfo outputDirectory)
        {
            RazorEngineService = razorEngineService;
            ContentDirectory = contentDirectory;
            OutputDirectory = outputDirectory;
        }
    }
}
