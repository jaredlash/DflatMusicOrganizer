using Dflat.ViewModels.DialogViewModels;
using NUnit.Framework;
using System;

namespace Tests.ViewModels.Unit_Tests
{
    [TestFixture]
    public class FileSourceFolderEditorViewModelTests
    {
        FileSourceFolderEditorViewModel editor;

        [SetUp]
        public void TestInitialize()
        {
            editor = new FileSourceFolderEditorViewModel(null, null, null, FileSourceFolderEditorMode.Edit);
        }

        #region Setting path
        [Test]
        public void ChoosePathCommand_OpensFolderChooserDialog()
        {
            throw new NotImplementedException();
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
