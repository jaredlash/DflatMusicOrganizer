using Moq;
using Dflat.Business;
using NUnit.Framework;

namespace Dflat.ViewModels.Tests
{
    [TestFixture]
    public class FileSourceManagerViewModelTests
    {
        [Test]
        public void CloseCommand_DisposesUnitOfWorkLifetimeManager()
        {
            var mockUnitOfWorkLifetimeManager = new Mock<IUnitOfWorkLifetimeManager>();
            var fsmvm = new FileSourceManagerViewModel(mockUnitOfWorkLifetimeManager.Object);

            fsmvm.CloseCommand.Execute(null);

            mockUnitOfWorkLifetimeManager.Verify(m => m.Dispose(), Times.Once());
        }

        [Test]
        public void SaveCommand_WhenThereAreChanges_SavesChangesOnUnitOfWork()
        {

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(m => m.HasChanges()).Returns(true);

            var uow = mockUnitOfWork.Object;

            var mockUnitOfWorkLifetimeManager = new Mock<IUnitOfWorkLifetimeManager>();
            mockUnitOfWorkLifetimeManager.SetupGet(m => m.UnitOfWork).Returns(uow);
            

            var fsmvm = new FileSourceManagerViewModel(mockUnitOfWorkLifetimeManager.Object);

            fsmvm.SaveCommand.Execute(null);

            mockUnitOfWork.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Test]
        public void SaveCommand_WhenThereAreNoChanges_DoesNotSaveChangesOnUnitOfWork()
        {

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(m => m.HasChanges()).Returns(false);

            var uow = mockUnitOfWork.Object;

            var mockUnitOfWorkLifetimeManager = new Mock<IUnitOfWorkLifetimeManager>();
            mockUnitOfWorkLifetimeManager.SetupGet(m => m.UnitOfWork).Returns(uow);


            var fsmvm = new FileSourceManagerViewModel(mockUnitOfWorkLifetimeManager.Object);

            fsmvm.SaveCommand.Execute(null);

            mockUnitOfWork.Verify(m => m.SaveChanges(), Times.Never());
        }
    }
}