using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Models
{
    public class FileChromaprintJob : Job
    {
        public File File { get; set; }
        public int FileID { get; set; }

        public override bool SameRequestAs(Job otherJob)
        {
            var compareJob = otherJob as FileChromaprintJob;
            if (compareJob == null)
                return false;

            return compareJob.FileID == FileID;
        }
    }
}
