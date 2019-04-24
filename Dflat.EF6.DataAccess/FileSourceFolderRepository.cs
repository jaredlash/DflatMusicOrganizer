using Dflat.Business.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dflat.Business.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Dflat.EF6.DataAccess
{
    public class FileSourceFolderRepository : IFileSourceFolderRepository
    {
        private readonly DataContext context;

        public FileSourceFolderRepository(DataContext context)
        {
            this.context = context;
        }

        public void Add(FileSourceFolder folder)
        {
            context.FileSourceFolders.Add(folder);
        }

        
        public FileSourceFolder Get(int id)
        {
            var folder = context.FileSourceFolders.Find(id);
            if (folder != null)
                context.Entry(folder).Collection(f => f.ExcludePaths).Load();

            return folder;
        }

        public List<FileSourceFolder> GetAll()
        {
            return context.FileSourceFolders
                .Include(p => p.ExcludePaths)
                .OrderBy(p => p.Path)
                .ToList();
        }

        public void Remove(FileSourceFolder folder)
        {
            context.FileSourceFolders.Remove(folder);
        }

        public void Remove(int id)
        {
            // If tracked, set as deleted
            var tracked = context.ChangeTracker.Entries<FileSourceFolder>().Any(p => p.Entity.FileSourceFolderID == id);
            if (tracked)
                context.FileSourceFolders.Remove(context.FileSourceFolders.Find(id));
            else // Otherwise, remove by a dummy folder
            {
                var folderToRemove = new FileSourceFolder { FileSourceFolderID = id };
                context.FileSourceFolders.Attach(folderToRemove);
                context.FileSourceFolders.Remove(folderToRemove);
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
