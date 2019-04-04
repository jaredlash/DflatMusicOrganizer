using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Models
{
    public class ExcludePath
    {
        public int ExcludePathID { get; set; }
        public int FileSourceFolderID { get; set; }
        public string Path { get; set; }
    }
}
