﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dflat.Business.Models
{
    public class FileSourceFolderScanJob : Job
    {
        public int FileSourceFolderID { get; set; }
        public FileSourceFolder FileSourceFolder { get; set; }

        public FileSourceFolderScanJob()
        {

        }

        public FileSourceFolderScanJob(int FileSourceFolderID)
        {
            this.FileSourceFolderID = FileSourceFolderID;
        }

        public override bool SameRequestAs(Job otherJob)
        {
            if (!(otherJob is FileSourceFolderScanJob compareJob))
                return false;

            return compareJob.FileSourceFolderID == FileSourceFolderID;
        }
    }
}
