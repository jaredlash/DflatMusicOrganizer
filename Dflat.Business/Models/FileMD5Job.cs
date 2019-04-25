using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Models
{
    public class FileMD5Job : Job
    {
        public File File { get; set; }
        public int FileID { get; set; }

        public override bool SameRequestAs(Job otherJob)
        {
            var compareJob = otherJob as FileMD5Job;
            if (compareJob == null)
                return false;

            return FileID == compareJob.FileID;
        }
    }
}
