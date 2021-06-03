using System.IO;
using System.Linq;

namespace BlogToHtml.Commands.BuildBlog.Generators
{
    public class TemplateContext
    {
        public DirectoryInfo Root { get; }

        public FileInfo OutputFile { get; }

        public string ReleativeRootPath
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

        public TemplateContext(DirectoryInfo root, FileInfo outputFile)
        {
            Root = root;
            OutputFile = outputFile;
        }
    }
}