using Dflat.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.Application.Repositories
{
    public interface IFileRepository
    {
        File Get(int fileID);

        IEnumerable<File> GetFromPath(string path, bool recurse = true);
        
        // Sets FileID on the added file
        void Add(File newFile);
        void Update(File modifiedFile);

        void UpdateMD5(int fileID, string md5);

        void MarkRemoved(int fileID);
    }
}
