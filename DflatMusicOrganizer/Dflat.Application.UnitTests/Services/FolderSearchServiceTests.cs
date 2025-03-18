using Dflat.Application.Services;
using Dflat.Application.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Dflat.Application.UnitTests.Services.Tests
{


    [TestClass]
    public class FolderSearchServiceTests
    {
        #region Constructor

        [TestMethod]
        public void FolderSearchService_ReturnsFiveFiles_WhenNoConditionSupplied()
        {
            var fileList = new List<string>
            {
                @"Z:\file1.mp3",
                @"Z:\file2.mp3",
                @"Z:\file3.ogg",
                @"Z:\file4.ogg",
                @"Z:\file5.ogg"
            };
            var sysio = new Mock<ISystemIOWrapper>();
            var mockFileInfo = new Mock<IFileInfo>();
            mockFileInfo.Setup(f => f.DirectoryName).Returns("");
            mockFileInfo.Setup(f => f.Name).Returns("");
            mockFileInfo.Setup(f => f.Extension).Returns("");
            mockFileInfo.Setup(f => f.Length).Returns(0);
            mockFileInfo.Setup(f => f.LastWriteTime).Returns(DateTime.Now);
            sysio.Setup(s => s.DirectoryExists(It.Is<string>(p => p == @"Z:\"))).Returns(true);
            sysio.Setup(s => s.GetDirectories(It.Is<string>(p => p == @"Z:\"))).Returns(Array.Empty<string>());
            sysio.Setup(s => s.GetFiles(It.Is<string>(p => p == @"Z:\"))).Returns(fileList.ToArray());
            sysio.Setup(s => s.GetFileInfo(It.IsAny<string>())).Returns(mockFileInfo.Object);

            var folderSearchService = new FolderSearchService(sysio.Object);

            var results = folderSearchService.FindFiles(@"Z:\", new CancellationToken());


            Assert.AreEqual(5, results.FoundFiles.Count);
        }

        [TestMethod]
        public void FolderSearchService_ReturnsThreeFiles_WhenConditionSupplied()
        {
            var fileList = new List<string>
            {
                @"Z:\file1.mp3",
                @"Z:\file2.mp3",
                @"Z:\file3.ogg",    // Meets criteria
                @"Z:\file4.ogg",    // Meets criteria
                @"Z:\file5.ogg"     // Meets criteria
            };
            var sysio = new Mock<ISystemIOWrapper>();
            var mockFileInfo = new Mock<IFileInfo>();
            mockFileInfo.Setup(f => f.DirectoryName).Returns("");
            mockFileInfo.Setup(f => f.Name).Returns("");
            mockFileInfo.Setup(f => f.Extension).Returns("");
            mockFileInfo.Setup(f => f.Length).Returns(0);
            mockFileInfo.Setup(f => f.LastWriteTime).Returns(DateTime.Now);
            sysio.Setup(s => s.DirectoryExists(It.Is<string>(p => p == @"Z:\"))).Returns(true);
            sysio.Setup(s => s.GetDirectories(It.Is<string>(p => p == @"Z:\"))).Returns(Array.Empty<string>());
            sysio.Setup(s => s.GetFiles(It.Is<string>(p => p == @"Z:\"))).Returns(fileList.ToArray());
            sysio.Setup(s => s.GetFileInfo(It.IsAny<string>())).Returns(mockFileInfo.Object);

            var folderSearchService = new FolderSearchService(sysio.Object, f => f.EndsWith(".ogg"));   // Criteria supplied in constructor

            var results = folderSearchService.FindFiles(@"Z:\", new CancellationToken());


            Assert.AreEqual(3, results.FoundFiles.Count);
        }
        #endregion

        #region FindFiles
        [TestMethod]
        public void FindFiles_ReturnsFiveFiles_WhenNoConditionSupplied()
        {
            var fileList = new List<string>
            {
                @"Z:\file1.mp3",
                @"Z:\file2.mp3",
                @"Z:\file3.ogg",
                @"Z:\file4.ogg",
                @"Z:\file5.ogg"
            };
            var sysio = new Mock<ISystemIOWrapper>();
            var mockFileInfo = new Mock<IFileInfo>();
            mockFileInfo.Setup(f => f.DirectoryName).Returns("");
            mockFileInfo.Setup(f => f.Name).Returns("");
            mockFileInfo.Setup(f => f.Extension).Returns("");
            mockFileInfo.Setup(f => f.Length).Returns(0);
            mockFileInfo.Setup(f => f.LastWriteTime).Returns(DateTime.Now);
            sysio.Setup(s => s.DirectoryExists(It.Is<string>(p => p == @"Z:\"))).Returns(true);
            sysio.Setup(s => s.GetDirectories(It.Is<string>(p => p == @"Z:\"))).Returns(Array.Empty<string>());
            sysio.Setup(s => s.GetFiles(It.Is<string>(p => p == @"Z:\"))).Returns(fileList.ToArray());
            sysio.Setup(s => s.GetFileInfo(It.IsAny<string>())).Returns(mockFileInfo.Object);

            var folderSearchService = new FolderSearchService(sysio.Object);

            var results = folderSearchService.FindFiles(@"Z:\", new CancellationToken());


            Assert.AreEqual(5, results.FoundFiles.Count);
        }

        [TestMethod]
        public void FindFiles_ReturnsThreeFiles_WhenConditionSupplied()
        {
            var fileList = new List<string>
            {
                @"Z:\file1.mp3",
                @"Z:\file2.mp3",
                @"Z:\file3.ogg",    // Meets criteria
                @"Z:\file4.ogg",    // Meets criteria
                @"Z:\file5.ogg"     // Meets criteria
            };
            var sysio = new Mock<ISystemIOWrapper>();
            var mockFileInfo = new Mock<IFileInfo>();
            mockFileInfo.Setup(f => f.DirectoryName).Returns("");
            mockFileInfo.Setup(f => f.Name).Returns("");
            mockFileInfo.Setup(f => f.Extension).Returns("");
            mockFileInfo.Setup(f => f.Length).Returns(0);
            mockFileInfo.Setup(f => f.LastWriteTime).Returns(DateTime.Now);
            sysio.Setup(s => s.DirectoryExists(It.Is<string>(p => p == @"Z:\"))).Returns(true);
            sysio.Setup(s => s.GetDirectories(It.Is<string>(p => p == @"Z:\"))).Returns(Array.Empty<string>());
            sysio.Setup(s => s.GetFiles(It.Is<string>(p => p == @"Z:\"))).Returns(fileList.ToArray());
            sysio.Setup(s => s.GetFileInfo(It.IsAny<string>())).Returns(mockFileInfo.Object);

            var folderSearchService = new FolderSearchService(sysio.Object, f => f.EndsWith(".ogg"));   // Criteria supplied in constructor

            var results = folderSearchService.FindFiles(@"Z:\", new CancellationToken());


            Assert.AreEqual(3, results.FoundFiles.Count);
        }



        [TestMethod]
        public void FindFiles_ReturnsZeroFiles_RootFolderExcluded()
        {
            var fileList = new List<string>
            {
                @"Z:\file1.mp3",
                @"Z:\file2.mp3",
                @"Z:\file3.ogg",
                @"Z:\file4.ogg",
                @"Z:\file5.ogg"
            };
            var sysio = new Mock<ISystemIOWrapper>();
            var mockFileInfo = new Mock<IFileInfo>();
            mockFileInfo.Setup(f => f.DirectoryName).Returns("");
            mockFileInfo.Setup(f => f.Name).Returns("");
            mockFileInfo.Setup(f => f.Extension).Returns("");
            mockFileInfo.Setup(f => f.Length).Returns(0);
            mockFileInfo.Setup(f => f.LastWriteTime).Returns(DateTime.Now);
            sysio.Setup(s => s.DirectoryExists(It.Is<string>(p => p == @"Z:\"))).Returns(true);
            sysio.Setup(s => s.GetDirectories(It.Is<string>(p => p == @"Z:\"))).Returns(Array.Empty<string>());
            sysio.Setup(s => s.GetFiles(It.Is<string>(p => p == @"Z:\"))).Returns(fileList.ToArray());
            sysio.Setup(s => s.GetFileInfo(It.IsAny<string>())).Returns(mockFileInfo.Object);

            var folderSearchService = new FolderSearchService(sysio.Object);

            var results = folderSearchService.FindFiles(@"Z:\", new HashSet<string> { @"Z:\" }, (f) => true, new CancellationToken());   // Root folder excluded


            Assert.AreEqual(0, results.FoundFiles.Count);
        }


        [TestMethod]
        public void FindFiles_ThrowsDirectoryNotFoundException_WhenDirectoryDoesNotExist()
        {
            var fileList = new List<string>
            {
                @"Z:\file1.mp3",
                @"Z:\file2.mp3",
                @"Z:\file3.ogg",
                @"Z:\file4.ogg",
                @"Z:\file5.ogg"
            };
            var sysio = new Mock<ISystemIOWrapper>();
            var mockFileInfo = new Mock<IFileInfo>();
            mockFileInfo.Setup(f => f.DirectoryName).Returns("");
            mockFileInfo.Setup(f => f.Name).Returns("");
            mockFileInfo.Setup(f => f.Extension).Returns("");
            mockFileInfo.Setup(f => f.Length).Returns(0);
            mockFileInfo.Setup(f => f.LastWriteTime).Returns(DateTime.Now);
            sysio.Setup(s => s.DirectoryExists(It.Is<string>(p => p == @"Z:\"))).Returns(false);                    // Specify that directory is not found
            sysio.Setup(s => s.GetDirectories(It.Is<string>(p => p == @"Z:\"))).Returns(Array.Empty<string>());
            sysio.Setup(s => s.GetFiles(It.Is<string>(p => p == @"Z:\"))).Returns(fileList.ToArray());
            sysio.Setup(s => s.GetFileInfo(It.IsAny<string>())).Returns(mockFileInfo.Object);

            var folderSearchService = new FolderSearchService(sysio.Object);

            Assert.ThrowsException<DirectoryNotFoundException>(() => folderSearchService.FindFiles(@"Z:\", new CancellationToken()));
        }


        [TestMethod]
        public void FindFiles_FindsFiveNestedFiles_WhenFiveFilesAreNested()
        {
            //fileList.Add(@"Z:\dir1\file1.mp3");
            //fileList.Add(@"Z:\dir1\file2.mp3");
            //fileList.Add(@"Z:\dir1\file3.ogg");
            //fileList.Add(@"Z:\dir2\file4.ogg");
            //fileList.Add(@"Z:\dir2\dir3\file5.ogg");
            Dictionary<string, List<string>> subDirs = new Dictionary<string, List<string>>
            {
                { @"Z:\", new List<string> { @"Z:\dir1", @"Z:\dir2" } },
                { @"Z:\dir1", new List<string>() },
                { @"Z:\dir2", new List<string> { @"Z:\dir2\dir3" } },
                { @"Z:\dir2\dir3", new List<string>() }
            };
            Dictionary<string, List<string>> files = new Dictionary<string, List<string>>
            {
                { @"Z:\", new List<string>()  },
                { @"Z:\dir1", new List<string> { @"Z:\dir1\file1.mp3", @"Z:\dir1\file2.mp3", @"Z:\dir1\file3.ogg"} },
                { @"Z:\dir2", new List<string> { @"Z:\dir2\file4.ogg" } },
                { @"Z:\dir2\dir3", new List<string> { @"Z:\dir2\dir3\file5.ogg" } }
            };
            var sysio = new Mock<ISystemIOWrapper>();
            var mockFileInfo = new Mock<IFileInfo>();
            mockFileInfo.Setup(f => f.DirectoryName).Returns("");
            mockFileInfo.Setup(f => f.Name).Returns("");
            mockFileInfo.Setup(f => f.Extension).Returns("");
            mockFileInfo.Setup(f => f.Length).Returns(0);
            mockFileInfo.Setup(f => f.LastWriteTime).Returns(DateTime.Now);
            sysio.Setup(s => s.DirectoryExists(It.IsAny<string>())).Returns((string p) => subDirs.TryGetValue(p, out _));
            sysio.Setup(s => s.GetDirectories(It.IsAny<string>())).Returns((string p) => subDirs[p].ToArray());
            sysio.Setup(s => s.GetFiles(It.IsAny<string>())).Returns((string p) => files[p].ToArray());
            sysio.Setup(s => s.GetFileInfo(It.IsAny<string>())).Returns(mockFileInfo.Object);

            var folderSearchService = new FolderSearchService(sysio.Object);

            var results = folderSearchService.FindFiles(@"Z:\", new CancellationToken());


            Assert.AreEqual(5, results.FoundFiles.Count);
        }

        [TestMethod]
        public void FindFiles_FindsThreeNestedFiles_WhenNoMP3PredicateUsed()
        {
            //fileList.Add(@"Z:\dir1\file1.mp3");
            //fileList.Add(@"Z:\dir1\file2.mp3");
            //fileList.Add(@"Z:\dir1\file3.ogg");           // Meets criteria
            //fileList.Add(@"Z:\dir2\file4.ogg");           // Meets criteria
            //fileList.Add(@"Z:\dir2\dir3\file5.ogg");      // Meets criteria
            Dictionary<string, List<string>> subDirs = new Dictionary<string, List<string>>
            {
                { @"Z:\", new List<string> { @"Z:\dir1", @"Z:\dir2" } },
                { @"Z:\dir1", new List<string>() },
                { @"Z:\dir2", new List<string> { @"Z:\dir2\dir3" } },
                { @"Z:\dir2\dir3", new List<string>() }
            };
            Dictionary<string, List<string>> files = new Dictionary<string, List<string>>
            {
                { @"Z:\", new List<string>()  },
                { @"Z:\dir1", new List<string> { @"Z:\dir1\file1.mp3", @"Z:\dir1\file2.mp3", @"Z:\dir1\file3.ogg"} },
                { @"Z:\dir2", new List<string> { @"Z:\dir2\file4.ogg" } },
                { @"Z:\dir2\dir3", new List<string> { @"Z:\dir2\dir3\file5.ogg" } }
            };
            var sysio = new Mock<ISystemIOWrapper>();
            var mockFileInfo = new Mock<IFileInfo>();
            mockFileInfo.Setup(f => f.DirectoryName).Returns("");
            mockFileInfo.Setup(f => f.Name).Returns("");
            mockFileInfo.Setup(f => f.Extension).Returns("");
            mockFileInfo.Setup(f => f.Length).Returns(0);
            mockFileInfo.Setup(f => f.LastWriteTime).Returns(DateTime.Now);
            sysio.Setup(s => s.DirectoryExists(It.IsAny<string>())).Returns((string p) => subDirs.TryGetValue(p, out _));
            sysio.Setup(s => s.GetDirectories(It.IsAny<string>())).Returns((string p) => subDirs[p].ToArray());
            sysio.Setup(s => s.GetFiles(It.IsAny<string>())).Returns((string p) => files[p].ToArray());
            sysio.Setup(s => s.GetFileInfo(It.IsAny<string>())).Returns(mockFileInfo.Object);

            var folderSearchService = new FolderSearchService(sysio.Object);

            var results = folderSearchService.FindFiles(@"Z:\", (p) => p.EndsWith(".mp3") == false, new CancellationToken());    // Criteria provided in call to FindFiles


            Assert.AreEqual(3, results.FoundFiles.Count);
        }

        [TestMethod]
        public void FindFiles_FindsTwoFilesWithOneError_WhenSubdir1ThrowsException()
        {
            //fileList.Add(@"Z:\dir1\file1.mp3");       // Throw on dir1
            //fileList.Add(@"Z:\dir1\file2.mp3");       // ^^
            //fileList.Add(@"Z:\dir1\file3.ogg");       // ^^
            //fileList.Add(@"Z:\dir2\file4.ogg");       // Found
            //fileList.Add(@"Z:\dir2\dir3\file5.ogg");  // Found
            Dictionary<string, List<string>> subDirs = new Dictionary<string, List<string>>
            {
                { @"Z:\", new List<string> { @"Z:\dir1", @"Z:\dir2" } },
                { @"Z:\dir1", new List<string>() },
                { @"Z:\dir2", new List<string> { @"Z:\dir2\dir3" } },
                { @"Z:\dir2\dir3", new List<string>() }
            };
            Dictionary<string, List<string>> files = new Dictionary<string, List<string>>
            {
                { @"Z:\", new List<string>()  },
                { @"Z:\dir1", new List<string> { @"Z:\dir1\file1.mp3", @"Z:\dir1\file2.mp3", @"Z:\dir1\file3.ogg"} },
                { @"Z:\dir2", new List<string> { @"Z:\dir2\file4.ogg" } },
                { @"Z:\dir2\dir3", new List<string> { @"Z:\dir2\dir3\file5.ogg" } }
            };
            var sysio = new Mock<ISystemIOWrapper>();
            var mockFileInfo = new Mock<IFileInfo>();
            mockFileInfo.Setup(f => f.DirectoryName).Returns("");
            mockFileInfo.Setup(f => f.Name).Returns("");
            mockFileInfo.Setup(f => f.Extension).Returns("");
            mockFileInfo.Setup(f => f.Length).Returns(0);
            mockFileInfo.Setup(f => f.LastWriteTime).Returns(DateTime.Now);
            sysio.Setup(s => s.DirectoryExists(It.IsAny<string>())).Returns((string p) => subDirs.TryGetValue(p, out _));
            sysio.Setup(s => s.GetDirectories(It.IsAny<string>())).Returns((string p) => subDirs[p].ToArray());
            sysio.Setup(s => s.GetFiles(It.IsAny<string>())).Returns((string p) => files[p].ToArray());
            sysio.Setup(s => s.GetFiles(It.Is<string>((p) => p == @"Z:\dir1"))).Throws(new UnauthorizedAccessException("Permission denied")); // Dir1 throw
            sysio.Setup(s => s.GetFileInfo(It.IsAny<string>())).Returns(mockFileInfo.Object);

            var folderSearchService = new FolderSearchService(sysio.Object);

            var results = folderSearchService.FindFiles(@"Z:\", new CancellationToken());


            Assert.AreEqual(2, results.FoundFiles.Count);
            Assert.AreEqual(1, results.ErrorLog.Count);
        }



        [TestMethod]
        public void FindFiles_FindsFourFilesWithOneError_WhenFile1ThrowsException()
        {
            //fileList.Add(@"Z:\dir1\file1.mp3");   // Exception thrown
            //fileList.Add(@"Z:\dir1\file2.mp3");
            //fileList.Add(@"Z:\dir1\file3.ogg");
            //fileList.Add(@"Z:\dir2\file4.ogg");
            //fileList.Add(@"Z:\dir2\dir3\file5.ogg");
            Dictionary<string, List<string>> subDirs = new Dictionary<string, List<string>>
            {
                { @"Z:\", new List<string> { @"Z:\dir1", @"Z:\dir2" } },
                { @"Z:\dir1", new List<string>() },
                { @"Z:\dir2", new List<string> { @"Z:\dir2\dir3" } },
                { @"Z:\dir2\dir3", new List<string>() }
            };
            Dictionary<string, List<string>> files = new Dictionary<string, List<string>>
            {
                { @"Z:\", new List<string>()  },
                { @"Z:\dir1", new List<string> { @"Z:\dir1\file1.mp3", @"Z:\dir1\file2.mp3", @"Z:\dir1\file3.ogg"} },
                { @"Z:\dir2", new List<string> { @"Z:\dir2\file4.ogg" } },
                { @"Z:\dir2\dir3", new List<string> { @"Z:\dir2\dir3\file5.ogg" } }
            };
            var sysio = new Mock<ISystemIOWrapper>();
            var mockFileInfo = new Mock<IFileInfo>();
            mockFileInfo.Setup(f => f.DirectoryName).Returns("");
            mockFileInfo.Setup(f => f.Name).Returns("");
            mockFileInfo.Setup(f => f.Extension).Returns("");
            mockFileInfo.Setup(f => f.Length).Returns(0);
            mockFileInfo.Setup(f => f.LastWriteTime).Returns(DateTime.Now);
            sysio.Setup(s => s.DirectoryExists(It.IsAny<string>())).Returns((string p) => subDirs.TryGetValue(p, out _));
            sysio.Setup(s => s.GetDirectories(It.IsAny<string>())).Returns((string p) => subDirs[p].ToArray());
            sysio.Setup(s => s.GetFiles(It.IsAny<string>())).Returns((string p) => files[p].ToArray());
            sysio.Setup(s => s.GetFileInfo(It.IsAny<string>())).Returns(mockFileInfo.Object);
            sysio.Setup(s => s.GetFileInfo(It.Is<string>((p) => p == @"Z:\dir1\file1.mp3"))).Throws(new UnauthorizedAccessException("Test")); // File1 exception

            var folderSearchService = new FolderSearchService(sysio.Object);

            var results = folderSearchService.FindFiles(@"Z:\", new CancellationToken());


            Assert.AreEqual(4, results.FoundFiles.Count);
            Assert.AreEqual(1, results.ErrorLog.Count);
        }
        #endregion
    }
}