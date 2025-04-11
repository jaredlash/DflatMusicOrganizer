using FileService.Common;
using FileService.Responses;

namespace FileService.Services;

public interface IDirectoryService
{
    Result<IEnumerable<DirectoryItem>, string> ListDirectoryContents(RelativePath? relativePath);
}
