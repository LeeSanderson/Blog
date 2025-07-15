using System.IO.Abstractions;
using System;

namespace BlogToHtml;

internal static class FileSystemExtensions
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

    public static IFileInfo ToFileInfo(this IFileSystem fileSystem, string? path, bool errorIfNotExists = true)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));

        var result = fileSystem.FileInfo.New(path);
        if (errorIfNotExists && !result.Exists)
        {
            throw new Exception($"File {path} does not exist");
        }

        return result;
    }
}