using Moq;
using Dflat.Business;
using NUnit.Framework;
using Dflat.ViewModels.Dialogs;
using System.ComponentModel;
using Dflat.Business.Models;
using System.Collections.Generic;
using Dflat.Business.Repositories;

namespace Dflat.ViewModels.Tests
{
    [TestFixture]
    public class FileSourceManagerViewModelTests
    {
        #region Set up mocked FileSourceManager

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

        #endregion

        #region Test that unit of work lifetime manager is disposed

        [Test]
        public void CloseCommand_DisposesUnitOfWorkLifetimeManager()
        {
            
            var mockUnitOfWorkLifetimeManager = new Mock<IUnitOfWorkLifetimeManager>();
            var fsmvm = CreateFileSourceManagerViewModel(mockUnitOfWorkLifetimeManager.Object, null, null, null);

            fsmvm.CloseCommand.Execute(null);

            mockUnitOfWorkLifetimeManager.Verify(m => m.Dispose(), Times.Once());
        }

        #endregion

        #region Test Save Command

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

        #endregion

        #region Test Add Command
        
        [Test]
        public void AddCommand_OpensFileSourceFolderEditor()
        {
            var dummyRepo = new List<FileSourceFolder>();

            var mockUnitOfWork = new Mock<IUnitOfWork>();


            var mockFileSourceFolderRepository = new Mock<IFileSourceFolderRepository>();
            mockFileSourceFolderRepository.Setup(m => m.Create()).Returns(new FileSourceFolder());

            var fileSourceFolderRepository = mockFileSourceFolderRepository.Object;

            mockUnitOfWork.SetupGet(m => m.IFileSourceFolderRepository).Returns(fileSourceFolderRepository);
            var uow = mockUnitOfWork.Object;

            var mockUnitOfWorkLifetimeManager = new Mock<IUnitOfWorkLifetimeManager>();
            mockUnitOfWorkLifetimeManager.SetupGet(m => m.UnitOfWork).Returns(uow);

            var mockDialogService = new Mock<IDialogService>();

            var fsmvm = CreateFileSourceManagerViewModel(mockUnitOfWorkLifetimeManager.Object, null, mockDialogService.Object, null);

            fsmvm.AddCommand.Execute(null);

            mockDialogService.Verify(m => m.FileSourceFolderEditor(It.IsNotNull<IUnitOfWorkLifetimeManager>(), It.IsNotNull<FileSourceFolder>()), Times.Once());
        }


        #endregion

        #region Test Edit Command
        // Edit
        #endregion

        #region Test Remove Command
        // Remove
        #endregion

        #region Test Cancel Command (Request Close)

        [Test]
        public void RequestCloseCommand_ClosesView()
        {
            var mockIVew = new Mock<IView>();

            var fsmvm = CreateFileSourceManagerViewModel(null, null, null, null);

            fsmvm.RequestClose.Execute(mockIVew.Object);



            mockIVew.Verify(m => m.Close(), Times.Once());
        }

        #endregion

        #region Test Closing

        [Test]
        public void ClosingCommand_WhenThereAreUnsavedChanges_PromptsToConfirmClose()
        {
            var mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(m => m.ConfirmDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(m => m.HasChanges()).Returns(true);

            var uow = mockUnitOfWork.Object;

            var mockUnitOfWorkLifetimeManager = new Mock<IUnitOfWorkLifetimeManager>();
            mockUnitOfWorkLifetimeManager.SetupGet(m => m.UnitOfWork).Returns(uow);
            
            var fsmvm = CreateFileSourceManagerViewModel(mockUnitOfWorkLifetimeManager.Object, null, mockDialogService.Object, null);

            var cancelEventArgs = new CancelEventArgs();
            cancelEventArgs.Cancel = false;

            fsmvm.ClosingCommand.Execute(cancelEventArgs);


            mockDialogService.Verify(m => m.ConfirmDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }


        [Test]
        public void ClosingCommand_WhenThereAreUnsavedChangesAndUserCancelsAtPrompt_CancelsClosing()
        {
            var mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(m => m.ConfirmDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);   // User cancels when prompted

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(m => m.HasChanges()).Returns(true);

            var uow = mockUnitOfWork.Object;

            var mockUnitOfWorkLifetimeManager = new Mock<IUnitOfWorkLifetimeManager>();
            mockUnitOfWorkLifetimeManager.SetupGet(m => m.UnitOfWork).Returns(uow);

            var fsmvm = CreateFileSourceManagerViewModel(mockUnitOfWorkLifetimeManager.Object, null, mockDialogService.Object, null);

            var cancelEventArgs = new CancelEventArgs();
            cancelEventArgs.Cancel = false;

            fsmvm.ClosingCommand.Execute(cancelEventArgs);


            Assert.IsTrue(cancelEventArgs.Cancel);
        }


        [Test]
        public void ClosingCommand_WhenThereAreUnsavedChangesAndUserConfirmsAtPrompt_ContinuesClosing()
        {
            var mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(m => m.ConfirmDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);   // User confirms when prompted

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(m => m.HasChanges()).Returns(true);

            var uow = mockUnitOfWork.Object;

            var mockUnitOfWorkLifetimeManager = new Mock<IUnitOfWorkLifetimeManager>();
            mockUnitOfWorkLifetimeManager.SetupGet(m => m.UnitOfWork).Returns(uow);

            var fsmvm = CreateFileSourceManagerViewModel(mockUnitOfWorkLifetimeManager.Object, null, mockDialogService.Object, null);

            var cancelEventArgs = new CancelEventArgs();
            cancelEventArgs.Cancel = false;

            fsmvm.ClosingCommand.Execute(cancelEventArgs);


            Assert.IsFalse(cancelEventArgs.Cancel);
        }


        [Test]
        public void ClosingCommand_WhenThereAreNoChanges_DoesNotPromptToConfirmClose()
        {
            var mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(m => m.ConfirmDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(m => m.HasChanges()).Returns(false);

            var uow = mockUnitOfWork.Object;

            var mockUnitOfWorkLifetimeManager = new Mock<IUnitOfWorkLifetimeManager>();
            mockUnitOfWorkLifetimeManager.SetupGet(m => m.UnitOfWork).Returns(uow);


            var fsmvm = CreateFileSourceManagerViewModel(mockUnitOfWorkLifetimeManager.Object, null, mockDialogService.Object, null);

            var cancelEventArgs = new CancelEventArgs();
            cancelEventArgs.Cancel = false;

            fsmvm.ClosingCommand.Execute(cancelEventArgs);


            mockDialogService.Verify(m => m.ConfirmDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }


        #endregion
    }
}