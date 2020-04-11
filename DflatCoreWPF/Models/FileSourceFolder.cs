using System;
using System.Collections.Generic;
using System.Text;

namespace DflatCoreWPF.Models
{
    public class FileSourceFolder
    {
        public int FileSourceFolderID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsTemporaryMedia { get; set; }
        public DateTime? LastScanStart { get; set; }
        public bool IsChanged { get; set; }
        public ICollection<ExcludePath> ExcludePaths { get; set; } = new HashSet<ExcludePath>();
    }
}
