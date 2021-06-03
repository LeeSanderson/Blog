namespace BlogToHtml.Commands.BuildBlog
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    internal static class DirectoryInfoExtensions
    {
        public static DirectoryInfo ToDirectoryInfo(this string? path, bool createIfNotExists = false)
        {
            var result = new DirectoryInfo(path);
            if (!result.Exists && createIfNotExists)
            {
                result.Create();
                result = new DirectoryInfo(path);
            }

            if (!result.Exists)
            {
                throw new Exception($"Directory {path} does not exist");
            }

            return result;
        }

        public static async Task RecurseAsync(this DirectoryInfo directoryInfo, Func<FileInfo, Task> onFile, Func<DirectoryInfo, Task>? onDirectory = null)
        {
            foreach (var fileInfo in directoryInfo.EnumerateFiles())
            {
                await onFile(fileInfo);
            }

            foreach (var childDirectoryInfo in directoryInfo.EnumerateDirectories())
            {
                // Recurse child directory first (incase we are trying to delete the directory.
                await childDirectoryInfo.RecurseAsync(onFile, onDirectory);
                if (onDirectory != null)
                {
                    await onDirectory (childDirectoryInfo);
                }
            }
        }
    }
}
