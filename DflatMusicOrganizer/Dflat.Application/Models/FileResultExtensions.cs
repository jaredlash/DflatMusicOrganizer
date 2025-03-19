using System;

namespace Dflat.Application.Models;

public static class FileResultExtensions
{
    public static File ToNewFile(this FileResult fileResult) => new File
    {
        FileID = Guid.NewGuid(),
        Filename = fileResult.Filename,
        Extension = fileResult.Extension,
        Directory = fileResult.Directory,
        Size = fileResult.Size,
        LastModifiedTime = fileResult.LastModifiedTime
    };
}
