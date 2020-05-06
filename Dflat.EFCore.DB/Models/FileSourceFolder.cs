using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.Data.EFCore.Models
{
    public class FileSourceFolder
    {
        public FileSourceFolder()
        {
            ExcludePaths = new HashSet<ExcludePath>();
        }
        public int FileSourceFolderID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsTemporaryMedia { get; set; }
        public DateTime? LastScanStart { get; set; }

        public ICollection<ExcludePath> ExcludePaths { get; set; }
    }
}
