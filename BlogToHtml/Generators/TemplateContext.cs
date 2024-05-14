using System.IO;
using BlogToHtml.Models;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;

namespace BlogToHtml.Generators
{
    public class TemplateContext
    {
        public IDirectoryInfo Root { get; }

        public IFileInfo OutputFile { get; }

        // ReSharper disable once UnusedMember.Global
        // [Used in templates]
        public string RelativeRootPath
        {
            get
            {
                var result = "./";
                var dirsBetweenRootAndOutput = OutputFile.FullName.Replace(Root.FullName, "").Count(c => c == '\\') - 1;
                if (dirsBetweenRootAndOutput > 0)
                    result += string.Concat(Enumerable.Repeat("../", dirsBetweenRootAndOutput));
                return result;
            }
        }

        public TemplateContext(IDirectoryInfo root, IFileInfo outputFile)
        {
            Root = root;
            OutputFile = outputFile;
        }

        // ReSharper disable once UnusedMember.Global
        // [Used in templates]
        public string Url => OutputFile.GetFullUrl(Root);

        // ReSharper disable once UnusedMember.Global
        // [Used in templates]
        public string HeroImageUrl => Path.ChangeExtension(OutputFile.GetFullUrl(Root), ".png");

        // ReSharper disable once UnusedMember.Global
        // [Used in templates]
        public string RelativeUrlTo(SummaryModel model)
        {
            var currentDirectory = OutputFile!.Directory!;
            return model.OutputFileInfo!.RelativeUrlTo(currentDirectory);
        }
    }
}