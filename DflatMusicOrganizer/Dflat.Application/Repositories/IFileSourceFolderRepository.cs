using Dflat.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dflat.Application.Repositories;

public interface IFileSourceFolderRepository
{
    IEnumerable<FileSourceFolder> GetAll();

    Task<IEnumerable<FileSourceFolder>> GetAllAsync();

    Task<bool> UpdateAllAsync(IEnumerable<FileSourceFolder> fileSourceFolders);

    Task AddOrUpdateAsync(FileSourceFolder fileSourceFolder);

    FileSourceFolder Get(int fileSourceFolderID);

    /// <summary>
    /// Sets LastScanTime to the current time
    /// </summary>
    /// <param name="fileSourceFolderID"></param>
    Task UpdateLastScanTimeAsync(int fileSourceFolderID);
}
