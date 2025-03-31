using System.Collections.Generic;

namespace Dflat.Application.Models;

public class FileSourceFolderScanJob : Job
{
    public int FileSourceFolderID { get; init; }

    public IList<File> FilesNeedingMD5s { get; set; } = new List<File>();

    public FileSourceFolderScanJob()
    {

    }

    public FileSourceFolderScanJob(int FileSourceFolderID)
    {
        this.FileSourceFolderID = FileSourceFolderID;
    }

    public override bool SameRequestAs(Job otherJob)
    {
        if (otherJob is not FileSourceFolderScanJob compareJob)
            return false;

        return compareJob.FileSourceFolderID == FileSourceFolderID;
    }

    public override string ToString()
    {
        return "File Source Folder Scan";
    }
}
