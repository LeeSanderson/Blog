using System;
using System.IO;
using System.Threading.Tasks;

namespace BlogToHtml.Commands.BuildBlog.Generators
{
    internal class EmbeddedContentGenerator : GeneratorBase, IEmbeddedContentGenerator
    {
        private readonly string[] resourceFiles;

        public EmbeddedContentGenerator(GeneratorContext generatorContext, params string[] resourceFiles): base(generatorContext) 
        {
            this.resourceFiles = resourceFiles;
        }

        public async Task GenerateContentAsync()
        {
            foreach (var resourceFile in resourceFiles)
            {
                var assembly = typeof(EmbeddedContentGenerator).Assembly;
                var resourceName = $"BlogToHtml.StaticResources.{resourceFile}";
                await using var stream = assembly.GetManifestResourceStream(resourceName) ??
                                         throw new Exception($"Resource {resourceName} not found");

                var dummyResourceFileName = Path.Combine(GeneratorContext.ContentDirectory.FullName, resourceFile);
                var outputFileName = GetOutputFileName(dummyResourceFileName);
                var outputFileInfo = GeneratorContext.FileSystem.FileInfo.New(outputFileName);

                await using var writer = outputFileInfo.Create();
                await stream.CopyToAsync(writer);
            }
        }
    }
}
