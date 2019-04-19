using Dflat.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dflat.Business.Repositories;

namespace Dflat.EF6.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext context;
        private bool disposed;

        private readonly IFileSourceFolderRepository fileSourceFolderRepository;
        private readonly IJobRepository jobRepository;

        public UnitOfWork() : this(new DataContext())
        {

        }

        public UnitOfWork(DataContext context)
        {
            this.context = context;

            this.disposed = false;

            fileSourceFolderRepository = new FileSourceFolderRepository(context);
            jobRepository = new JobRepository(context);
            
        }

        public IFileSourceFolderRepository FileSourceFolderRepository
        {
            get
            {
                return fileSourceFolderRepository;
            }
        }

        public IJobRepository JobRepository
        {
            get
            {
                return jobRepository;
            }
        }

        public bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }





        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed || !disposing)
                return;

            if (context != null)
                context.Dispose();

            disposed = true;
        }

        #endregion
    }
}
