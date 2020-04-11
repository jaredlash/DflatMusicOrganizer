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
            if (!(otherJob is FileMD5Job compareJob))
                return false;

            return FileID == compareJob.FileID;
        }
    }
}
