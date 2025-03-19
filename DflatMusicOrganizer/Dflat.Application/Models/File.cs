using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Application.Models
{
    public class File
    {
        public File()
        {
            Filename = string.Empty;
            Extension = string.Empty;
            Directory = string.Empty;
            MD5Sum = string.Empty;
        }

        public File(Guid id, string filename, string extension, string directory, long size, DateTime lastModifiedTime)
        {
            FileID = id;
            Filename = filename;
            Extension = extension;
            Directory = directory;
            Size = size;
            LastModifiedTime = lastModifiedTime;
        }


        public Guid FileID { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public string Directory { get; set; }
        public long Size { get; set; }
        public DateTime LastModifiedTime { get; set; }

        public string MD5Sum { get; set; }
        // TODO: Put this in a FileAudio model: public string Chromaprint { get; set; }
    }
}
