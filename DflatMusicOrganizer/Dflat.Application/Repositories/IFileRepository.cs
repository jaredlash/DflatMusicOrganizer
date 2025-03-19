using Dflat.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.Application.Repositories
{
    public interface IFileRepository
    {
        File Get(Guid fileID);

        /// <summary>
        /// Returns all files from the specified path
        /// </summary>
        /// <param name="path">Path to search</param>
        /// <param name="excludePaths">Any sub-directories to exclude from results. Only valid with recurse = true</param>
        /// <param name="recurse">Whether to include files in sub-directories</param>
        /// <returns></returns>
        IEnumerable<File> GetFromPath(string path, IEnumerable<string> excludePaths = null, bool recurse = true);
        
        // Sets FileID on the added file
        void Add(File newFile);
        void Update(File modifiedFile);

        void UpdateMD5(Guid fileID, string md5);

        void MarkRemoved(Guid fileID);
    }
}
