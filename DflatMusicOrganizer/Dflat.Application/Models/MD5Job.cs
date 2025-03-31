using System;

namespace Dflat.Application.Models;

public class MD5Job : Job
{
    public Guid FileID { get; init; }

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
