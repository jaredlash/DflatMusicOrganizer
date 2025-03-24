using Dflat.Application.Models;

namespace Dflat.Application.UnitTests.Services.JobServices;

public class TestJob : Job
{
    public override bool SameRequestAs(Job otherJob)
    {
        return Description == otherJob.Description;
    }
}
