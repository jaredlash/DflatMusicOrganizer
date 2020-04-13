using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.EFCore.DB.Models
{
    public class FileSourceFolderData
    {
        public FileSourceFolderData()
        {
            ExcludePaths = new HashSet<ExcludePathData>();
        }
        public int FileSourceFolderID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsTemporaryMedia { get; set; }
        public DateTime? LastScanStart { get; set; }

        public ICollection<ExcludePathData> ExcludePaths { get; set; }
    }
}
