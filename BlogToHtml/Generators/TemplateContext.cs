using BlogToHtml.Models;
using System.IO.Abstractions;
using System.Linq;

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
        public string RelativeUrlTo(SummaryModel model)
        {
            var outputPath = OutputFile!.Directory!.FullName;
            var relativePath = model.OutputFileInfo!.FullName.Replace(outputPath, "./");
            return relativePath.Replace('\\', '/');
        }
    }
}