﻿using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.EFCore.DB.Repositories
{
    public class FileRepository : IFileRepository
    {
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