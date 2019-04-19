using Dflat.Business.Repositories;
using System;

namespace Dflat.Business
{
    public interface IUnitOfWork : IDisposable
    {
        IFileSourceFolderRepository FileSourceFolderRepository { get;  }
        IJobRepository JobRepository { get; }

        bool HasChanges();

        void SaveChanges();
    }
}
