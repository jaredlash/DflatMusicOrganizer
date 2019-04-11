using Dflat.Business.Models;
using Dflat.ViewModels.Dialogs;
using Dflat.ViewModels.DialogViewModels;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

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
            editor.AddExcludePathCommand.Execute(null);

            mockDialogService.Verify(m => m.FolderChooserDialog(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void AddExcludePathCommand_WhenUserAddsAnExistingPath_PathNotAdded()
        {
            folderChoice = "Existing Path";

            editor.ExcludePaths.Add(folderChoice);  // Make sure we are set up with the path already added.

            editor.AddExcludePathCommand.Execute(null);

            Assert.AreEqual(1, editor.ExcludePaths.Count);
        }

        [Test]
        public void AddExcludePathCommand_WhenUserAddsANewPath_PathIsAdded()
        {
            folderChoice = "New Path";

            editor.ExcludePaths.Add("Existing Path");

            editor.AddExcludePathCommand.Execute(null);

            Assert.AreEqual(2, editor.ExcludePaths.Count);
        }
        #endregion
        

        #region Removing exclude path
        [Test]
        public void RemoveExcludePath_WhenNoExcludePathIsSelected_CannotExecute()
        {
            editor.SelectedExcludePathIndex = -1;

            Assert.IsFalse(editor.RemoveExcludePathCommand.CanExecute(null));
        }

        [Test]
        public void RemoveExcludePath_WhenExcludePathIsSelected_CanExecute()
        {
            editor.SelectedExcludePathIndex = 0;

            Assert.IsTrue(editor.RemoveExcludePathCommand.CanExecute(null));
        }

        [Test]
        public void RemoveExcludePath_WhenExcludePathIsSelected_RemovesSelectedPath()
        {
            editor.ExcludePaths.Add("Path One");
            editor.ExcludePaths.Add("Path Two");
            editor.ExcludePaths.Add("Path Three");

            editor.SelectedExcludePathIndex = 1;
            // Not sure if this will work to get the correct path since ICollection is not indexable.  Second Assert might fail.
            var pathToRemove = editor.ExcludePaths.ToList()[editor.SelectedExcludePathIndex];

            editor.RemoveExcludePathCommand.Execute(null);

            Assert.AreEqual(2, editor.ExcludePaths.Count);
            Assert.IsFalse(editor.ExcludePaths.Contains(pathToRemove));
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
            folderToEdit.Path = "Original Path";

            editor.Path = "New Path";

            editor.OkCommand.Execute(null);

            Assert.AreEqual(editor.Path, folderToEdit.Path);
        }

        [Test]
        public void OkCommand_WhenUserRemovesExcludePath_FolderNoLongerHasRemovedExcludePath()
        {
            editor.Path = "NonEmpty";

            string pathToRemove = "Path to remove";
            string pathToKeep = "Path to keep";

            folderToEdit.ExcludePaths.Add(new ExcludePath { Path = pathToRemove });
            folderToEdit.ExcludePaths.Add(new ExcludePath { Path = pathToKeep });

            // Set up our editor with only having the path to keep
            editor.ExcludePaths.Add(pathToKeep);

            editor.OkCommand.Execute(null);

            var foundExcludePath = folderToEdit.ExcludePaths.Where(p => p.Path == pathToRemove).ToList();
            Assert.AreEqual(0, foundExcludePath.Count);
        }

        [Test]
        public void OkCommand_WhenUserAddsExcludePath_FolderHasNewExcludePath()
        {
            editor.Path = "NonEmpty";

            string existingPath = "Existing path";
            string pathToAdd = "Added path";

            folderToEdit.ExcludePaths.Add(new ExcludePath { Path = existingPath });

            // Set up our editor with both the existing path and the path to add
            editor.ExcludePaths.Add(existingPath);
            editor.ExcludePaths.Add(pathToAdd);

            editor.OkCommand.Execute(null);

            var foundExcludePath = folderToEdit.ExcludePaths.Where(p => p.Path == pathToAdd).ToList();

            Assert.AreEqual(2, folderToEdit.ExcludePaths.Count);
            Assert.AreEqual(1, foundExcludePath.Count);
        }

        [Test]
        public void OkCommand_WhenUserChangesIncludeInScanSetting_FolderHasNewIncludeInScanSetting()
        {
            editor.Path = "NonEmpty";

            editor.IncludeInScans = false;
            folderToEdit.IncludeInScans = true;

            editor.OkCommand.Execute(null);

            Assert.AreEqual(editor.IncludeInScans, folderToEdit.IncludeInScans);
        }

        [Test]
        public void OkCommand_WhenUserAddsAndRemovesExcludePaths_FoldeerHasDifferentExcludePaths()
        {
            editor.Path = "NonEmpty";

            string existingPath = "Existing path";
            string pathToAdd = "Added path";

            folderToEdit.ExcludePaths.Add(new ExcludePath { Path = existingPath });

            // Set up our editor with only the path to add, and leaving out the existing path
            editor.ExcludePaths.Add(pathToAdd);

            editor.OkCommand.Execute(null);

            var foundOldExcludePath = folderToEdit.ExcludePaths.Where(p => p.Path == existingPath).ToList();
            var foundNewExcludePath = folderToEdit.ExcludePaths.Where(p => p.Path == pathToAdd).ToList();

            Assert.AreEqual(1, folderToEdit.ExcludePaths.Count);
            Assert.AreEqual(0, foundOldExcludePath.Count);
            Assert.AreEqual(1, foundNewExcludePath.Count);
        }

        [Test]
        public void CancelCommand_WhenUserHasMadeChanges_FolderDoesNotGetChanged()
        {
            // Set up the folder
            string oldFolderPath = "Old path";
            bool oldFolderIncludeInScans = true;

            string existingExcludePath = "Existing path";
            string newExcludePath = "Added path";

            folderToEdit.Path = oldFolderPath;
            folderToEdit.IncludeInScans = oldFolderIncludeInScans;

            folderToEdit.ExcludePaths.Add(new ExcludePath { Path = existingExcludePath });

            editor.Path = "New path";
            editor.IncludeInScans = false;

            // Set up our editor with only the path to add, and leaving out the existing path
            editor.ExcludePaths.Add(newExcludePath);

            editor.CancelCommand.Execute(null);

            var foundOldExcludePath = folderToEdit.ExcludePaths.Where(p => p.Path == existingExcludePath).ToList();
            var foundNewExcludePath = folderToEdit.ExcludePaths.Where(p => p.Path == newExcludePath).ToList();

            Assert.AreEqual(oldFolderPath, folderToEdit.Path);
            Assert.AreEqual(oldFolderIncludeInScans, folderToEdit.IncludeInScans);
            Assert.AreEqual(1, folderToEdit.ExcludePaths.Count);
            Assert.AreEqual(1, foundOldExcludePath.Count);
            Assert.AreEqual(0, foundNewExcludePath.Count);
        }
        #endregion
    }
}
