using Moq;
using Dflat.Business;
using NUnit.Framework;
using Dflat.ViewModels.Dialogs;
using System.ComponentModel;
using Dflat.Business.Models;
using System.Collections.Generic;
using Dflat.Business.Repositories;
using Dflat.ViewModels.DialogViewModels;

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
        private IDialogService dialogService;

        private FileSourceManagerViewModel fileSourceManagerViewModel;

        private bool hasChanges;
        private bool userConfirmsOperation;
        private bool userAcceptsNewFolder;

        #region Set up system under test

        [SetUp]
        public void TestInitialize()
        {
            dummyRepo = new List<IFileSourceFolder>();

            // Set up our mock FileSourceFolderRepository
            mockFileSourceFolderRepository = new Mock<IFileSourceFolderRepository>();
            mockFileSourceFolderRepository.Setup(m => m.Create()).Returns(() => dummyRepoCreate());
            mockFileSourceFolderRepository.Setup(m => m.Remove(It.IsNotNull<FileSourceFolder>())).Callback<FileSourceFolder>((f) => dummyRepoRemove(f));

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
            mockDialogService.Setup(m => m.ConfirmDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(() => userConfirmsOperation);
            mockDialogService.Setup(m => m.FileSourceFolderEditor(It.IsNotNull<IUnitOfWorkLifetimeManager>(), It.IsNotNull<FileSourceFolder>(), It.IsAny<FileSourceFolderEditorMode>())).Returns(() => userAcceptsNewFolder);
            dialogService = mockDialogService.Object;


            fileSourceManagerViewModel = new FileSourceManagerViewModel(uowLifetimeManager, null, dialogService, null);

        }

        private FileSourceFolder dummyRepoCreate()
        {
            var f = new FileSourceFolder();
            dummyRepo.Add(f);
            return f;
        }

        private void dummyRepoRemove(FileSourceFolder folder)
        {
            dummyRepo.Remove(folder);
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

        [Test]
        public void SaveCommand_WhenThereAreChanges_SaveCommandCanExecute()
        {
            hasChanges = true;

            Assert.IsTrue(fileSourceManagerViewModel.SaveCommand.CanExecute(null));
        }


        [Test]
        public void SaveCommand_WhenThereAreNoChanges_SaveCommandCanotExecute()
        {
            hasChanges = false;

            Assert.IsFalse(fileSourceManagerViewModel.SaveCommand.CanExecute(null));
        }

        #endregion

        #region Test Add Command

        [Test]
        public void AddCommand_OpensFileSourceFolderEditor()
        {


            fileSourceManagerViewModel.AddCommand.Execute(null);

            mockDialogService.Verify(m => m.FileSourceFolderEditor(It.IsNotNull<IUnitOfWorkLifetimeManager>(), It.IsNotNull<FileSourceFolder>(), It.Is<FileSourceFolderEditorMode>((p) => p == FileSourceFolderEditorMode.New)), Times.Once());
        }

        [Test]
        public void AddCommand_CreatesNewFileSourceFolder()
        {
            fileSourceManagerViewModel.AddCommand.Execute(null);

            mockFileSourceFolderRepository.Verify(m => m.Create(), Times.Once());
        }

        [Test]
        public void AddCommand_WhenUserFinishesFolderEditor_AddsNewFileSourceFolder()
        {
            userAcceptsNewFolder = true;

            fileSourceManagerViewModel.AddCommand.Execute(null);

            Assert.AreEqual(1, dummyRepo.Count);
        }

        [Test]
        public void AddCommand_WhenUserCancelsFolderEditor_DoesNotAddNewFileSourceFolder()
        {
            userAcceptsNewFolder = false;


            fileSourceManagerViewModel.AddCommand.Execute(null);

            Assert.AreEqual(0, dummyRepo.Count);
        }

        [Test]
        public void AddCommand_RaiseCanExecuteChangedForSaveCommand()
        {
            bool canExecuteChangeExecuted = false;

            fileSourceManagerViewModel.SaveCommand.CanExecuteChanged += (e, a) => canExecuteChangeExecuted = true;

            fileSourceManagerViewModel.AddCommand.Execute(null);

            Assert.IsTrue(canExecuteChangeExecuted);

        }

        #endregion

        #region Test Edit Command

        [Test]
        public void EditCommand_OpensFileSourceFolderEditor()
        {
            var fileSourceFolder = new FileSourceFolder();

            dummyRepo.Add(fileSourceFolder);

            fileSourceManagerViewModel.SelectedFileSourceFolder = fileSourceFolder;

            fileSourceManagerViewModel.EditCommand.Execute(null);

            mockDialogService.Verify(m => m.FileSourceFolderEditor(It.IsNotNull<IUnitOfWorkLifetimeManager>(), It.IsNotNull<FileSourceFolder>(), It.Is<FileSourceFolderEditorMode>((p) => p == FileSourceFolderEditorMode.Edit)), Times.Once());
        }

        [Test]
        public void EditCommand_DoesNotCreateNewFileSourceFolder()
        {
            fileSourceManagerViewModel.EditCommand.Execute(null);

            mockFileSourceFolderRepository.Verify(m => m.Create(), Times.Never());
        }

        [Test]
        public void EditCommand_RaiseCanExecuteChangedForSaveCommand()
        {
            var fileSourceFolder = new FileSourceFolder();

            dummyRepo.Add(fileSourceFolder);
            fileSourceManagerViewModel.SelectedFileSourceFolder = fileSourceFolder;
            

            bool canExecuteChangeExecuted = false;
            fileSourceManagerViewModel.EditCommand.CanExecuteChanged += (e, a) => canExecuteChangeExecuted = true;


            fileSourceManagerViewModel.EditCommand.Execute(null);


            Assert.IsTrue(canExecuteChangeExecuted);

        }

        #endregion

        #region Test Remove Command

        [Test]
        public void RemoveCommand_PromptsUserToConfirmRemoval()
        {
            // Make sure we have a folder in our repo
            var fileSourceFolder = new FileSourceFolder();
            dummyRepo.Add(fileSourceFolder);

            fileSourceManagerViewModel.SelectedFileSourceFolder = fileSourceFolder;
            

            fileSourceManagerViewModel.RemoveCommand.Execute(null);

            mockDialogService.Verify(m => m.ConfirmDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }


        [Test]
        public void RemoveCommand_WhenUserConfirmsRemoval_RemovesSelectedFileSourceFolder()
        {
            // Make sure we have a folder in our repo
            var fileSourceFolder = new FileSourceFolder();
            dummyRepo.Add(fileSourceFolder);

            fileSourceManagerViewModel.SelectedFileSourceFolder = fileSourceFolder;

            userConfirmsOperation = true;
            
            fileSourceManagerViewModel.RemoveCommand.Execute(null);

            Assert.AreEqual(0, dummyRepo.Count);
        }

        [Test]
        public void RemoveCommand_WhenUserDeniesRemoval_RetainsSelectedFileSourceFolder()
        {
            // Make sure we have a folder in our repo
            var fileSourceFolder = new FileSourceFolder();
            dummyRepo.Add(fileSourceFolder);

            fileSourceManagerViewModel.SelectedFileSourceFolder = fileSourceFolder;

            userConfirmsOperation = false;

            fileSourceManagerViewModel.RemoveCommand.Execute(null);

            Assert.AreEqual(1, dummyRepo.Count);
        }

        [Test]
        public void RemoveCommand_RaiseCanExecuteChangedForSaveCommand()
        {
            // Make sure we have a folder in our repo
            var fileSourceFolder = new FileSourceFolder();
            dummyRepo.Add(fileSourceFolder);

            fileSourceManagerViewModel.SelectedFileSourceFolder = fileSourceFolder;

            userConfirmsOperation = true;

            bool canExecuteChangeExecuted = false;
            fileSourceManagerViewModel.EditCommand.CanExecuteChanged += (e, a) => canExecuteChangeExecuted = true;


            fileSourceManagerViewModel.RemoveCommand.Execute(null);

            Assert.IsTrue(canExecuteChangeExecuted);
        }

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
            userConfirmsOperation = true;

            var cancelEventArgs = new CancelEventArgs();
            cancelEventArgs.Cancel = false;

            fileSourceManagerViewModel.ClosingCommand.Execute(cancelEventArgs);


            mockDialogService.Verify(m => m.ConfirmDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }


        [Test]
        public void ClosingCommand_WhenThereAreUnsavedChangesAndUserCancelsAtPrompt_CancelsClosing()
        {
            hasChanges = true;
            userConfirmsOperation = false;

            var cancelEventArgs = new CancelEventArgs();
            cancelEventArgs.Cancel = false;

            fileSourceManagerViewModel.ClosingCommand.Execute(cancelEventArgs);


            Assert.IsTrue(cancelEventArgs.Cancel);
        }


        [Test]
        public void ClosingCommand_WhenThereAreUnsavedChangesAndUserConfirmsAtPrompt_ContinuesClosing()
        {
            hasChanges = true;
            userConfirmsOperation = true;

            var cancelEventArgs = new CancelEventArgs();
            cancelEventArgs.Cancel = false;

            fileSourceManagerViewModel.ClosingCommand.Execute(cancelEventArgs);


            Assert.IsFalse(cancelEventArgs.Cancel);
        }


        [Test]
        public void ClosingCommand_WhenThereAreNoChanges_DoesNotPromptToConfirmClose()
        {
            hasChanges = false;
            userConfirmsOperation = true;

            var cancelEventArgs = new CancelEventArgs();
            cancelEventArgs.Cancel = false;

            fileSourceManagerViewModel.ClosingCommand.Execute(cancelEventArgs);


            mockDialogService.Verify(m => m.ConfirmDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }


        #endregion
    }
}