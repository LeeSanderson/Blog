using System.IO.Abstractions;
using System.Linq;
using System.Net.Http;
using RazorEngine.Templating;

namespace BlogToHtml.Generators;

internal class GeneratorContext
{
    public IRazorEngineService RazorEngineService { get; }
    public IFileSystem FileSystem { get; }
    public IDirectoryInfo ContentDirectory { get; }
    public IDirectoryInfo OutputDirectory { get; }
    public IFileInfo[] ContentFiles { get; }
    public IHttpClientFactory HttpClientFactory { get;}

    public GeneratorContext(
        IRazorEngineService razorEngineService,
        IFileSystem fileSystem,
        IDirectoryInfo contentDirectory,
        IDirectoryInfo outputDirectory,
        IHttpClientFactory httpClientFactory)
    {
        RazorEngineService = razorEngineService;
        FileSystem = fileSystem;
        ContentDirectory = contentDirectory;
        OutputDirectory = outputDirectory;
        HttpClientFactory = httpClientFactory;
        ContentFiles = contentDirectory.Recurse().ToArray();
    }
}