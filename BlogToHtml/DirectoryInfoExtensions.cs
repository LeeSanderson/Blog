using System.Collections.Generic;
using System.IO.Abstractions;

namespace BlogToHtml
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public static class DirectoryInfoExtensions
    {
        public static IDirectoryInfo ToDirectoryInfo(this IFileSystem fileSystem, string? path, bool createIfNotExists = false)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));

            var result = fileSystem.DirectoryInfo.New(path);
            if (!result.Exists && createIfNotExists)
            {
                result.Create();
                result = fileSystem.DirectoryInfo.New(path);
            }

            if (!result.Exists)
            {
                throw new Exception($"Directory {path} does not exist");
            }

            return result;
        }

        private static async Task RecurseAsync(this IDirectoryInfo directoryInfo, Func<IFileInfo, Task> onFile, Func<IDirectoryInfo, Task>? onDirectory = null)
        {
            foreach (var fileInfo in directoryInfo.EnumerateFiles())
            {
                await onFile(fileInfo);
            }

            foreach (var childDirectoryInfo in directoryInfo.EnumerateDirectories())
            {
                // Recurse child directory first (in case we are trying to delete the directory).
                await childDirectoryInfo.RecurseAsync(onFile, onDirectory);
                if (onDirectory != null)
                {
                    await onDirectory(childDirectoryInfo);
                }
            }
        }

        public static IEnumerable<IFileInfo> Recurse(this IDirectoryInfo directoryInfo)
        {
            foreach (var fileInfo in directoryInfo.EnumerateFiles("*.*", SearchOption.AllDirectories))
            {
                yield return fileInfo;
            }
        }

        public static async Task CleanAsync(this IDirectoryInfo outputDirectory)
        {
            await outputDirectory.RecurseAsync(f => { f.Delete(); return Task.CompletedTask; }, d => { d.Delete(); return Task.CompletedTask; });
        }

    }
}
