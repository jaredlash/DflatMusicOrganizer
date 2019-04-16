using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Models
{
    public class FileSourceFolder : IFileSourceFolder
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
