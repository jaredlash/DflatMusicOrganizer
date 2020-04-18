using Dflat.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Application.Repositories
{
    public interface IFileSourceFolderRepository
    {
        IEnumerable<FileSourceFolder> GetAll();

        Task<IEnumerable<FileSourceFolder>> GetAllAsync();

        Task<bool> UpdateAllAsync(IEnumerable<FileSourceFolder> fileSourceFolders);

        FileSourceFolder Get(int fileSourceFolderID);
    }
}
