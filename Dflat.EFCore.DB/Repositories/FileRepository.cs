﻿using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.Data.EFCore.Repositories
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

        public void Add(File modifiedFile)
        {
            return;
        }

        public IEnumerable<File> GetFromPath(string path, bool recurse = true)
        {
            return new List<File>();
        }

        public void MarkRemoved(int fileID)
        {
            return;
        }

        public void Update(File modifiedFile)
        {
            return;
        }
    }
}
