using Dflat.Business.Models;
using Dflat.ViewModels.Dialogs;
using Dflat.ViewModels.DialogViewModels;
using Moq;
using NUnit.Framework;
using System;

namespace Tests.ViewModels.Unit_Tests
{
    [TestFixture]
    public class FileSourceFolderEditorViewModelTests
    {
        private Mock<IDialogService> mockDialogService;

        private IDialogService dialogService;

        private FileSourceFolderEditorViewModel editor;
        private FileSourceFolder folderToEdit;

        private string folderChoice;

        [SetUp]
        public void TestInitialize()
        {
            mockDialogService = new Mock<IDialogService>();
            mockDialogService.Setup(m => m.FolderChooserDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(() => folderChoice);

            dialogService = mockDialogService.Object;

            folderToEdit = new FileSourceFolder();

            editor = new FileSourceFolderEditorViewModel(null, folderToEdit, dialogService, FileSourceFolderEditorMode.Edit);
        }

        #region Setting path

        [Test]
        public void ChoosePathCommand_OpensFolderChooserDialog()
        {
            editor.ChoosePathCommand.Execute(null);

            mockDialogService.Verify(m => m.FolderChooserDialog(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ChoosePathCommand_WhenUserChoosesFolder_SetsPathToNewFolder()
        {
            editor.Path = string.Empty;

            folderChoice = "Folder Choice";

            editor.ChoosePathCommand.Execute(null);

            Assert.AreEqual(folderChoice, editor.Path);
        }

        [Test]
        public void ChoosePathCommand_WhenUserCancels_DoesNotChangePath()
        {
            editor.Path = string.Empty;

            folderChoice = null;

            Assert.AreNotEqual(folderChoice, editor.Path);
        }

        #endregion

        #region Adding exclude path
        [Test]
        public void AddExcludePathCommand_OpensFolderChooserDialog()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AddExcludePathCommand_WhenUserAddsAnExistingPath_PathNotAdded()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void AddExcludePathCommand_WhenUserAddsANewPath_PathIsAdded()
        {
            throw new NotImplementedException();
        }
        #endregion
        

        #region Removing exclude path
        [Test]
        public void RemoveExcludePath_WhenNoExcludePathIsSelected_CannotExecute()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void RemoveExcludePath_WhenExcludePathIsSelected_CanExecute()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void RemoveExcludePath_WhenExcludePathIsSelected_RemovesSelectedPath()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Closing editor
        [Test]
        public void OkCommand_WhenThereIsNoPath_UserCannotClickOK()
        {
            editor.Path = "";

            Assert.IsFalse(editor.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkCommand_WhenThereIsAPath_UserCanClickOK()
        {
            editor.Path = "NonEmpty";

            Assert.IsTrue(editor.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkCommand_WhenPathIsChanged_FolderHasNewPath()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void OkCommand_WhenUserRemovesExcludePath_FolderNoLongerHasRemovedExcludePath()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void OkCommand_WhenUserAddsExcludePath_FolderHasNewExcludePath()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void OkCommand_WhenUserChangesIncludeInScanSetting_FolderHasNewIncludeInScanSetting()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void OkCommand_WhenUserAddsAndRemovesExcludePaths_FoldeerHasDifferentExcludePaths()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void CancelCommand_WhenUserHasMadeChanges_FolderDoesNotGetChanged()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
