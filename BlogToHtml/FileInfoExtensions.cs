using System.IO.Abstractions;

namespace BlogToHtml
{
    internal static class FileInfoExtensions
    {
        public static string RelativeUrlTo(this IFileInfo file, IDirectoryInfo baseDirectory)
        {
            var outputPath = baseDirectory.FullName;
            var relativePath = file.FullName.Replace(outputPath, ".");
            return relativePath.Replace('\\', '/');
        }
    }
}
