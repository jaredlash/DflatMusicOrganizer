using FileService.Common;
using FileService.Infrastructure;
using FileService.Responses;
using System.Runtime.InteropServices;

namespace FileService.Services;

public class DirectoryService : IDirectoryService
{
    private readonly string _baseDirectory;
    private readonly IFileSystem _fileSystem;

    public DirectoryService(string baseDirectory, IFileSystem fileSystem) 
        => (_baseDirectory, _fileSystem) = (baseDirectory, fileSystem);
    
    public Result<IEnumerable<DirectoryItem>, string> ListDirectoryContents(RelativePath? relativePath)
    {
        string targetDirectory = _baseDirectory;

        if (relativePath?.PathComponents.Count > 0)
        {
            if (relativePath.PathComponents.Any(p => !IsValidPath(p)))
            {
                return Result<IEnumerable<DirectoryItem>, string>.Failure("Invalid path.");
            }

            targetDirectory = Path.Combine(_baseDirectory,
                Path.Combine(CollectionsMarshal.AsSpan(relativePath.PathComponents)));
        }

        if (!_fileSystem.DirectoryExists(targetDirectory))
        {
            return Result<IEnumerable<DirectoryItem>, string>.Failure("Directory not found.");
        }

        // TODO: This can probably throw an exception if the directory is not accessible
        var directoryItems = _fileSystem.EnumerateFileSystemEntries(targetDirectory)
            .Select(entry => new DirectoryItem(
                Name: Path.GetFileName(entry),
                Type: _fileSystem.DirectoryExists(Path.Combine(targetDirectory, entry)) ? "Directory" : "File"
                ));

        return Result<IEnumerable<DirectoryItem>, string>.Success(directoryItems);
    }

    private bool IsValidPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        if (path == "..")
        {
            return false;
        }

        if (path.Contains("/") || path.Contains("\\"))
        {
            return false;
        }

        return true;
    }
}
