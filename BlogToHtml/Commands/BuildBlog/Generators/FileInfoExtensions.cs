using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace BlogToHtml.Commands.BuildBlog.Generators
{
    internal static class FileInfoExtensions
    {
        public static string GetRelativePathTo(this IFileInfo source, IFileInfo target)
        {
            var sourcePathParts = source.DirectoryName!.Split(Path.DirectorySeparatorChar);
            var targetPathParts = target.DirectoryName!.Split(Path.DirectorySeparatorChar);

            var sharedPathParts = 0;
            for (var i = 0; i < sourcePathParts.Length && i < targetPathParts.Length; i++)
            {
                if (sourcePathParts[i].Equals(targetPathParts[i], StringComparison.OrdinalIgnoreCase))
                {
                    sharedPathParts++;
                }
            }

            bool sourceAndTargetAreInSameDirectory = sharedPathParts == sourcePathParts.Length &&
                                                     sharedPathParts == targetPathParts.Length;
            if (sourceAndTargetAreInSameDirectory)
            {
                return target.Name;
            }

            return string.Concat(Enumerable.Repeat("../", sourcePathParts.Length - sharedPathParts)) +
                   string.Join("/", targetPathParts.Skip(sharedPathParts)) + 
                   "/" + 
                   target.Name;
        }
    }
}
