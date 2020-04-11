using Dflat.Business.Factories;
using Dflat.Business.Models;
using Dflat.Business.Repositories;
using Dflat.Business.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Business.Unit_Tests
{
    [TestFixture]
    public class JobServiceTests
    {
        

        IUnitOfWorkFactory unitOfWorkFactory;
        IJobQueue jobQueue;
        IBackgroundJobRunner<Job> jobRunner;

        [SetUp]
        public void Initialize()
        {
            var mockJobRepository = new Mock<IJobRepository>();
            var mockUnitOfWorkFactory = new Mock<IUnitOfWorkFactory>();
            var mockJobQueue = new Mock<IJobQueue>();
            var mockJobRunner = new Mock<IBackgroundJobRunner<Job>>();

            unitOfWorkFactory = mockUnitOfWorkFactory.Object;
            jobQueue = mockJobQueue.Object;
            jobRunner = mockJobRunner.Object;
            var mockJobService = new Mock<JobService<Job>>(unitOfWorkFactory, jobQueue, jobRunner) { CallBase = true };


        }

        #region Run Jobs

        // RunJobs stops running jobs when MaxConcurrentJobCount is reached


        #endregion
    }
}
