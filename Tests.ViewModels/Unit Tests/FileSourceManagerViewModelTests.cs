using Moq;
using Dflat.Business;
using NUnit.Framework;
using Dflat.ViewModels.Dialogs;

namespace Dflat.ViewModels.Tests
{
    [TestFixture]
    public class FileSourceManagerViewModelTests
    {
        private FileSourceManagerViewModel CreateFileSourceManagerViewModel(IUnitOfWorkLifetimeManager uowManager, IViewService viewService, IDialogService dialogService, IViewModelFactory viewModelFactory)
        {
            if (uowManager == null)
            {
                var mockUnitOfWorkLifetimeManager = new Mock<IUnitOfWorkLifetimeManager>();
                uowManager = mockUnitOfWorkLifetimeManager.Object;
            }

            if (viewService == null)
            {
                var mockViewService = new Mock<IViewService>();
                viewService = mockViewService.Object;
            }
            
            if (dialogService == null)
            {
                var mockDialogService = new Mock<IDialogService>();
                dialogService = mockDialogService.Object;
            }
            
            if (viewModelFactory == null)
            {
                var mockViewModelFactory = new Mock<IViewModelFactory>();
                viewModelFactory = mockViewModelFactory.Object;
            }

            return new FileSourceManagerViewModel(uowManager, viewService, dialogService, viewModelFactory);
        }

        [Test]
        public void CloseCommand_DisposesUnitOfWorkLifetimeManager()
        {
            
            var mockUnitOfWorkLifetimeManager = new Mock<IUnitOfWorkLifetimeManager>();
            var fsmvm = CreateFileSourceManagerViewModel(mockUnitOfWorkLifetimeManager.Object, null, null, null);

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
            

            var fsmvm = CreateFileSourceManagerViewModel(mockUnitOfWorkLifetimeManager.Object, null, null, null);

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


            var fsmvm = CreateFileSourceManagerViewModel(mockUnitOfWorkLifetimeManager.Object, null, null, null);

            fsmvm.SaveCommand.Execute(null);

            mockUnitOfWork.Verify(m => m.SaveChanges(), Times.Never());
        }
    }
}