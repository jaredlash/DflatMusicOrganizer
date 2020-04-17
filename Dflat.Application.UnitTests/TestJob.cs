using Dflat.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dflat.Application.UnitTests
{
    public class TestJob : Job
    {
        public override bool SameRequestAs(Job otherJob)
        {
            return Description == otherJob.Description;
        }
    }
}
