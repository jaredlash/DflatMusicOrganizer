using System;

namespace Dflat.Application.Wrappers;

public interface IFileInfo
{
    string DirectoryName { get; }
    string Extension { get; }
    DateTime LastWriteTime { get; }
    long Length { get; }
    string Name { get; }
}