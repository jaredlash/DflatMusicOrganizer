namespace FileService.Infrastructure;

public interface IFileSystem
{
    bool DirectoryExists(string path);
    IEnumerable<string> EnumerateFileSystemEntries(string path);
}
