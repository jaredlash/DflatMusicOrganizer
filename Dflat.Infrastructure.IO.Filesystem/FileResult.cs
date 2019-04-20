using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Infrastructure.IO.Filesystem
{
    public class FileResult
    {
        /// <summary>
        /// Filename without the path
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Containing directory's full path (excluding the filename)
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// File extension
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Last modified time (system time, not UTC)
        /// </summary>
        public DateTime LastModifiedTime { get; set; }
        
    }
}
