using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Dflat.Infrastructure.IO.Filesystem
{
    public class FileResult
    {
        public FileResult(string filename, string[] directoryList, string directorySeparator, string extension, long size, DateTime lastModifiedTime)
        {
            Filename = filename;
            DirectoryList = directoryList;
            DirectorySeparator = directorySeparator;
            Extension = extension;
            Size = size;
            LastModifiedTime = lastModifiedTime;
        }


        /// <summary>
        /// Filename without the path
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Containing directory's full path (excluding the filename)
        /// </summary>
        public string Directory
        {
            get
            {
                return string.Join(DirectorySeparator, DirectoryList);
            }
        }
        

        public string[] DirectoryList { get; private set; }

        public string DirectorySeparator { get; private set; }

        /// <summary>
        /// File extension
        /// </summary>
        public string Extension { get; private set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long Size { get; private set; }

        /// <summary>
        /// Last modified time (system time, not UTC)
        /// </summary>
        public DateTime LastModifiedTime { get; private set; }
        
    }
}
