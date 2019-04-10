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
        private Mock<IFileSourceFolderRepository> mockFileSourceFolderRepository;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<IUnitOfWorkLifetimeManager> mockUnitOfWorkLifetimeManager;
        private Mock<IDialogService> mockDialogService;



        private List<IFileSourceFolder> dummyRepo;
        private IFileSourceFolderRepository fileSourceFolderRepository;
        private IUnitOfWork uow;
        private IUnitOfWorkLifetimeManager uowLifetimeManager;
        private IViewService viewService;
        private IDialogService dialogService;
        private IViewModelFactory viewModelFactory;

        private FileSourceManagerViewModel fileSourceManagerViewModel;

        private bool hasChanges;
        private bool userConfirmsClose;

        #region Set up system under test

        [SetUp]
        public void TestInitialize()
        {
            dummyRepo = new List<IFileSourceFolder>();

            // Set up our mock file source folder repository
            mockFileSourceFolderRepository = new Mock<IFileSourceFolderRepository>();
            mockFileSourceFolderRepository.Setup(m => m.Create()).Returns(new FileSourceFolder());

            // Set up our mock unit of work
            mockUnitOfWork = new Mock<IUnitOfWork>();


            fileSourceFolderRepository = mockFileSourceFolderRepository.Object;
            mockUnitOfWork.SetupGet(m => m.IFileSourceFolderRepository).Returns(fileSourceFolderRepository);
            mockUnitOfWork.Setup(m => m.HasChanges()).Returns(() => hasChanges);

            uow = mockUnitOfWork.Object;

            // Set up our UnitOfWorkLifetimeManager
            mockUnitOfWorkLifetimeManager = new Mock<IUnitOfWorkLifetimeManager>();
            mockUnitOfWorkLifetimeManager.SetupGet(m => m.UnitOfWork).Returns(uow);

            uowLifetimeManager = mockUnitOfWorkLifetimeManager.Object;


            // Set up our DialogService
            mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(m => m.ConfirmDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(() => userConfirmsClose);
            dialogService = mockDialogService.Object;


            fileSourceManagerViewModel = new FileSourceManagerViewModel(uowLifetimeManager, viewService, dialogService, viewModelFactory);

        }


        #endregion

        #region Test that unit of work lifetime manager is disposed

        [Test]
        public void CloseCommand_DisposesUnitOfWorkLifetimeManager()
        {

            fileSourceManagerViewModel.CloseCommand.Execute(null);

            mockUnitOfWorkLifetimeManager.Verify(m => m.Dispose(), Times.Once());
        }

        #endregion

        #region Test Save Command

        [Test]
        public void SaveCommand_WhenThereAreChanges_SavesChangesOnUnitOfWork()
        {
            hasChanges = true;

            fileSourceManagerViewModel.SaveCommand.Execute(null);

            mockUnitOfWork.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Test]
        public void SaveCommand_WhenThereAreNoChanges_DoesNotSaveChangesOnUnitOfWork()
        {
            hasChanges = false;

            fileSourceManagerViewModel.SaveCommand.Execute(null);

            mockUnitOfWork.Verify(m => m.SaveChanges(), Times.Never());
        }

        #endregion

        #region Test Add Command

        [Test]
        public void AddCommand_OpensFileSourceFolderEditor()
        {


            fileSourceManagerViewModel.AddCommand.Execute(null);

            mockDialogService.Verify(m => m.FileSourceFolderEditor(It.IsNotNull<IUnitOfWorkLifetimeManager>(), It.IsNotNull<FileSourceFolder>()), Times.Once());
        }

        [Test]
        public void AddCommand_CreatesNewFileSourceFolder()
        {
            fileSourceManagerViewModel.AddCommand.Execute(null);

            mockFileSourceFolderRepository.Verify(m => m.Create(), Times.Once());
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

            fileSourceManagerViewModel.RequestClose.Execute(mockIVew.Object);

            mockIVew.Verify(m => m.Close(), Times.Once());
        }

        #endregion

        #region Test Closing

        [Test]
        public void ClosingCommand_WhenThereAreUnsavedChanges_PromptsToConfirmClose()
        {

            hasChanges = true;
            userConfirmsClose = true;

            var cancelEventArgs = new CancelEventArgs();
            cancelEventArgs.Cancel = false;

            fileSourceManagerViewModel.ClosingCommand.Execute(cancelEventArgs);


            mockDialogService.Verify(m => m.ConfirmDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }


        [Test]
        public void ClosingCommand_WhenThereAreUnsavedChangesAndUserCancelsAtPrompt_CancelsClosing()
        {
            hasChanges = true;
            userConfirmsClose = false;

            var cancelEventArgs = new CancelEventArgs();
            cancelEventArgs.Cancel = false;

            fileSourceManagerViewModel.ClosingCommand.Execute(cancelEventArgs);


            Assert.IsTrue(cancelEventArgs.Cancel);
        }


        [Test]
        public void ClosingCommand_WhenThereAreUnsavedChangesAndUserConfirmsAtPrompt_ContinuesClosing()
        {
            hasChanges = true;
            userConfirmsClose = true;

            var cancelEventArgs = new CancelEventArgs();
            cancelEventArgs.Cancel = false;

            fileSourceManagerViewModel.ClosingCommand.Execute(cancelEventArgs);


            Assert.IsFalse(cancelEventArgs.Cancel);
        }


        [Test]
        public void ClosingCommand_WhenThereAreNoChanges_DoesNotPromptToConfirmClose()
        {
            hasChanges = false;
            userConfirmsClose = true;

            var cancelEventArgs = new CancelEventArgs();
            cancelEventArgs.Cancel = false;

            fileSourceManagerViewModel.ClosingCommand.Execute(cancelEventArgs);


            mockDialogService.Verify(m => m.ConfirmDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }


        #endregion
    }
}