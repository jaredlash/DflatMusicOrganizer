using Dflat.Business.Repositories;
using System;

namespace Dflat.Business
{
    public interface IUnitOfWork : IDisposable
    {
        IFileSourceFolderRepository IFileSourceFolderRepository { get;  }

        bool HasChanges();

        void SaveChanges();
    }
}
