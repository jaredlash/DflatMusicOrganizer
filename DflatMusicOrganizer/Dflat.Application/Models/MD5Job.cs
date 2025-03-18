using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.Application.Models
{
    public class MD5Job : Job
    {
        public int FileID { get; set; }

        public override bool SameRequestAs(Job otherJob)
        {
            if (!(otherJob is MD5Job compareJob))
                return false;

            return compareJob.FileID == FileID;
        }

        public override string ToString()
        {
            return "MD5 Job";
        }
    }
}
