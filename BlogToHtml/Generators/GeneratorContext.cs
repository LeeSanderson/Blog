using System.IO.Abstractions;
using System.Linq;
using RazorEngine.Templating;

namespace BlogToHtml.Generators
{
    public class GeneratorContext
    {
        public IRazorEngineService RazorEngineService { get; }
        public IFileSystem FileSystem { get; }
        public IDirectoryInfo ContentDirectory { get; }
        public IDirectoryInfo OutputDirectory { get; }
        public IFileInfo[] ContentFiles { get; }

        public GeneratorContext(
            IRazorEngineService razorEngineService,
            IFileSystem fileSystem,
            IDirectoryInfo contentDirectory,
            IDirectoryInfo outputDirectory)
        {
            RazorEngineService = razorEngineService;
            FileSystem = fileSystem;
            ContentDirectory = contentDirectory;
            OutputDirectory = outputDirectory;
            ContentFiles = contentDirectory.Recurse().ToArray();
        }
    }
}
