using System.Collections.Generic;
using Dflat.Business;
using Moq;
using Dflat.Business.Models;
using Dflat.Business.Repositories;
using NUnit.Framework;

namespace Tests.Business.Integration_Tests
{
    [TestFixture]
	public class FileSourceFolderTest
	{
        List<IFileSourceFolder> dummyRepo;
        IFileSourceFolderRepository fileSourceFolderRepository;
        IUnitOfWork unitOfWork;

        [SetUp]
        public void TestInitialize()
        {
            dummyRepo = new List<IFileSourceFolder>();

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            

            var mockFileSourceFolderRepository = new Mock<IFileSourceFolderRepository>();
            mockFileSourceFolderRepository.Setup(m => m.Create()).Returns(new FileSourceFolder());
            
            fileSourceFolderRepository = mockFileSourceFolderRepository.Object;

            mockUnitOfWork.SetupGet(m => m.FileSourceFolderRepository).Returns(fileSourceFolderRepository);

            unitOfWork = mockUnitOfWork.Object;

        }

		public FileSourceFolderTest ()
		{
		}


		[Test]
		public void TestCreateFromRepository()
		{
            IFileSourceFolder fileSourceFolder = unitOfWork.FileSourceFolderRepository.Create();
            

			Assert.IsNotNull(fileSourceFolder);
		}


       
	}
}

