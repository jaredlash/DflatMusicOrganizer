using System;
using System.IO;

namespace Dflat.Application.Wrappers;

// This needs to be revisited to ensure robustness
// exceptions are currently caught by the service using it.
public class FileInfoWrapper : IFileInfo
{
    private readonly FileInfo fileInfo;

    public FileInfoWrapper(string file)
    {
        fileInfo = new FileInfo(file);
    }

    public string Name => fileInfo.Name;

    public string DirectoryName => fileInfo.DirectoryName ?? string.Empty;

    public string Extension => fileInfo.Extension;

    public long Length => fileInfo.Length;

    public DateTime LastWriteTime => fileInfo.LastWriteTime;
}
