﻿using Dflat.Business.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dflat.Business.Models;

namespace Dflat.EF6.DataAccess
{
    public class FileRepository : IFileRepository
    {
        private readonly DataContext context;

        public FileRepository(DataContext context)
        {
            this.context = context;
        }

        public void Add(File file)
        {
            object _lock = new object();
            lock (_lock)
            {
                if (Contains(file))
                    throw new DuplicateFileException(file, "File already exists");

                context.Files.Add(file);
            }
        }

        public bool Contains(File file)
        {
            return context.Files.Any(f => f.Filename == file.Filename && f.Directory == file.Directory);
        }

        public File Get(int id)
        {
            return context.Files.Find(id);
        }

        public List<File> GetAll()
        {
            return context.Files.ToList();
        }

        public void Remove(int id)
        {
            throw new NotImplementedException();
        }

        public void Remove(File item)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
