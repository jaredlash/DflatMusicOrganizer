using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.Data.Dapper.Repositories
{
    public class FileRepository : IFileRepository
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly string connectionString;
#pragma warning restore IDE0052 // Remove unread private members

        public FileRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Add(File newFile)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<File> GetFromPath(string path, bool recurse = true)
        {
            throw new NotImplementedException();
        }

        public void MarkRemoved(int fileID)
        {
            throw new NotImplementedException();
        }

        public void Update(File modifiedFile)
        {
            throw new NotImplementedException();
        }
    }
}
