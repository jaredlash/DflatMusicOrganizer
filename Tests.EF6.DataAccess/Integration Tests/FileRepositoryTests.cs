using Dflat.Business.Models;
using Dflat.Business.Repositories;
using Dflat.EF6.DataAccess;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.EF6.DataAccess.Integration_Tests
{
    [TestFixture]
    class FileRepositoryTests : EFDataTest
    {
        FileRepository fileRepository;
        DataContext context;

        [SetUp]
        public override void TestInitialize()
        {
            base.TestInitialize();
            context = new DataContext();
            fileRepository = new FileRepository(context);
        }

        #region Adding file

        [Test]
        public void Add_WhenNoFilesInRepository_AddsFile()
        {
            File file = new File("test.mp3", ".mp3", "C:\\Test", 1000, DateTime.Now);

            fileRepository.Add(file);
            fileRepository.Save();

            int repoCount = context.Files.Count();

            Assert.AreNotEqual(0, file.FileID);
            Assert.AreEqual(1, repoCount);
        }

        [Test]
        public void Add_WhenExistingFilesInRepositoryThatDontMatch_AddsFile()
        {
            File file = new File("test.mp3", ".mp3", "C:\\Test", 1000, DateTime.Now);
            fileRepository.Add(file);
            fileRepository.Save();


            File fileToAdd = new File("test2.mp3", ".mp3", "C:\\TestOtherDir", 2000, DateTime.Now);
            fileRepository.Add(fileToAdd);
            fileRepository.Save();



            int repoCount = context.Files.Count();

            Assert.AreNotEqual(0, fileToAdd.FileID);
            Assert.AreEqual(2, repoCount);

        }

        [Test]
        public void Add_WhenExistingFileInRepositoryMatchesFileToAdd_RaisesDuplicateFileException()
        {
            File file = new File("test.mp3", ".mp3", "C:\\Test", 1000, DateTime.Now);
            fileRepository.Add(file);
            fileRepository.Save();


            File fileToAdd = new File(file.Filename, file.Extension, file.Directory, file.Size, file.LastModifiedTime);
            Assert.Throws<DuplicateFileException>(() => fileRepository.Add(fileToAdd));
        }

        #endregion

        #region Querying about files

        [Test]
        public void Contains_WhenExistingFileMatches_ReturnsTrue()
        {
            File file = new File("test.mp3", ".mp3", "C:\\Test", 1000, DateTime.Now);
            fileRepository.Add(file);
            fileRepository.Save();


            File fileToTest = new File(file.Filename, file.Extension, file.Directory, file.Size, file.LastModifiedTime);

            Assert.IsTrue(fileRepository.Contains(fileToTest));
        }


        [Test]
        public void Contains_WhenThereAreNoExistingFiles_ReturnsFalse()
        {
            File fileToTest = new File("test.mp3", ".mp3", "C:\\Test", 1000, DateTime.Now);
            
            Assert.IsFalse(fileRepository.Contains(fileToTest));
        }


        [Test]
        public void Contains_WhenExistingFilesDoNotMatch_ReturnsFalse()
        {
            File file = new File("test.mp3", ".mp3", "C:\\Test", 1000, DateTime.Now);
            fileRepository.Add(file);
            fileRepository.Save();


            File fileToTest = new File("otherfile.mp3", ".mp3", "C:\\OtherDir", 2000, DateTime.Now);

            Assert.IsFalse(fileRepository.Contains(fileToTest));
        }


        [Test]
        public void Contains_WhenExistingFileHasMatchingFilenameButNoMatchingDirectory_ReturnsFalse()
        {
            File file = new File("test.mp3", ".mp3", "C:\\Test", 1000, DateTime.Now);
            fileRepository.Add(file);
            fileRepository.Save();


            File fileToTest = new File(file.Filename, file.Extension, "C:\\OtherDir", file.Size, file.LastModifiedTime);

            Assert.IsFalse(fileRepository.Contains(fileToTest));
        }


        [Test]
        public void Contains_WhenExistingFileHasMatchingDirectoryButNoMatchingFilename_ReturnsFalse()
        {
            File file = new File("test.mp3", ".mp3", "C:\\Test", 1000, DateTime.Now);
            fileRepository.Add(file);
            fileRepository.Save();


            File fileToTest = new File("otherfilename.mp3", file.Extension, file.Directory, file.Size, file.LastModifiedTime);

            Assert.IsFalse(fileRepository.Contains(fileToTest));
        }

        #endregion

        #region Getting files

        [Test]
        public void Get_WhenFileIDExists_ReturnsFile()
        {
            int id;

            File file = new File("test.mp3", ".mp3", "C:\\Test", 1000, DateTime.Now);
            fileRepository.Add(file);
            fileRepository.Save();
            id = file.FileID;

            File tryToGet = fileRepository.Get(id);

            Assert.IsNotNull(tryToGet);
        }

        [Test]
        public void Get_WhenFileIDDoesNotExist_ReturnsNull()
        {
            int id;

            File file = new File("test.mp3", ".mp3", "C:\\Test", 1000, DateTime.Now);
            fileRepository.Add(file);
            fileRepository.Save();
            id = file.FileID;

            // Make this an ID that doesn't exist
            id++;

            File tryToGet = fileRepository.Get(id);

            Assert.IsNull(tryToGet);

        }

        [Test]
        public void GetAll_WhenThereAreNoFiles_ReturnsEmptyList()
        {
            var files = fileRepository.GetAll();

            Assert.IsNotNull(files);
            Assert.AreEqual(0, files.Count);
        }

        [Test]
        public void GetAll_WhenThereAreThreeFiles_ReturnsListWithThreeFiles()
        {
            File file1 = new File("test.mp3", ".mp3", "C:\\Test", 1000, DateTime.Now);
            File file2 = new File("test2.mp3", ".mp3", "C:\\TestOtherDir", 2000, DateTime.Now);
            File file3 = new File("test3.mp3", ".mp3", "C:\\TestOtherOtherDir", 4000, DateTime.Now);
            fileRepository.Add(file1);
            fileRepository.Add(file2);
            fileRepository.Add(file3);
            fileRepository.Save();

            var files = fileRepository.GetAll();

            Assert.IsNotNull(files);
            Assert.AreEqual(3, files.Count);
            Assert.IsTrue(files.Any((f) => f.FileID == file1.FileID));
            Assert.IsTrue(files.Any((f) => f.FileID == file2.FileID));
            Assert.IsTrue(files.Any((f) => f.FileID == file3.FileID));
        }

        #endregion

        #region Removing files

        #endregion
    }
}
