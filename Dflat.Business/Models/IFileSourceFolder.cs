using System;
using System.Collections.Generic;

namespace Dflat.Business.Models
{
    public interface IFileSourceFolder
    {
        int FileSourceFolderID { get; set; }

        string Name { get; set; }
        bool IncludeInScans { get; set; }
        ICollection<ExcludePath> ExcludePaths { get; set; }
        DateTime? LastScanStart { get; set; }
        string Path { get; set; }
    }
}