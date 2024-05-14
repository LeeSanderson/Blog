using System;
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

        public static string GetFullUrl(this IFileInfo file, IDirectoryInfo rootDirectory)
        {
            var relativeUrl = file.RelativeUrlTo(rootDirectory)[1..];
            return "https://www.sixsideddice.com/Blog" + relativeUrl;
        }
    }
}
