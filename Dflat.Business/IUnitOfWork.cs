using Dflat.Business.Repositories;
using System;

namespace Dflat.Business
{
    public interface IUnitOfWork : IDisposable
    {
        IFileRepository FileRepository { get; }

        IFileSourceFolderRepository FileSourceFolderRepository { get;  }
        IJobRepository JobRepository { get; }

        bool HasChanges();

        void SaveChanges();
    }
}
