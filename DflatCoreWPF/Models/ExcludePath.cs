using System;
using System.Collections.Generic;
using System.Text;

namespace DflatCoreWPF.Models
{
    public class ExcludePath
    {
        public int ExcludePathID { get; set; }
        public int FileSourceFolderID { get; set; }
        public string Path { get; set; }
    }
}
