using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Application.Models
{
    public class File : IComparable<File>, IEquatable<File>
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

        public static int Compare(File x, File y)
        {
            if (x.Directory != y.Directory) return string.Compare(x.Directory, y.Directory);
            if (x.Filename != y.Filename) return string.Compare(x.Filename, y.Filename);
            if (x.LastModifiedTime != y.LastModifiedTime) return x.LastModifiedTime.CompareTo(y.LastModifiedTime);
            if (x.Size != y.Size) return x.Size.CompareTo(y.Size);

            return 0;
        }

        public int CompareTo(File other)
        {
            return Compare(this, other);
        }

        public bool Equals(File other)
        {
            return this.CompareTo(other) == 0;
        }

        // TODO: Put this in a FileAudio model: public string Chromaprint { get; set; }

        public bool PathsEqual(File other)
        {
            return PathOnlyCompare(this, other) == 0;
        }

        public static int PathOnlyCompare(File x, File y)
        {
            if (x.Directory != y.Directory) return string.Compare(x.Directory, y.Directory);
            if (x.Filename != y.Filename) return string.Compare(x.Filename, y.Filename);

            return 0;
        }
    }
}
