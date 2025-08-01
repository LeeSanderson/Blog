﻿using System.Collections.Generic;
using System.IO.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlogToHtml;

internal static class DirectoryInfoExtensions
{
    public static IDirectoryInfo CreateIfNotExists(this IDirectoryInfo directoryInfo)
    {
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
            directoryInfo.Refresh();
        }

        if (!directoryInfo.Exists)
        {
            throw new Exception($"Directory {directoryInfo.FullName} does not exist");
        }

        return directoryInfo;
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
