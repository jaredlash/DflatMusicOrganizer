using Dflat.Business;
using Dflat.Business.Models;
using Dflat.Business.Repositories;
using Dflat.EF6.DataAccess;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Linq;

namespace Tests.EF6.DataAccess.Integration_Tests
{
    [TestFixture]
    public class FileSourceFolderRepositoryTests : EFDataTest
    {
        IFileSourceFolderRepository fileSourceFolderRepository;

        [SetUp]
        public override void TestInitialize()
        {
            base.TestInitialize();

            fileSourceFolderRepository = new FileSourceFolderRepository(new DataContext());
        }

        [Test]
        public void CreateAndSaveFileSourceFolderSetsID()
        {
            int id = 0;
            string test_path = @"\Create\And\Save\FileSourceFolder\Sets\ID";

            FileSourceFolder fileSourceFolder = new FileSourceFolder();
            fileSourceFolder.Path = test_path;

            // Add the new source folder to the DB and save.
            fileSourceFolderRepository.Add(fileSourceFolder);
            fileSourceFolderRepository.Save();

            id = fileSourceFolder.FileSourceFolderID;

            Assert.AreNotEqual(0, id);
        }


        [Test]
        public void CreateAndSaveFileSourceFolderWithNoExcludePathsSavesToDB()
        {
            int id;
            string test_path = @"\Create\And\Save\FileSourceFolder\With\No\Exclude\Paths\Saves\To\DB";

            FileSourceFolder fileSourceFolder = new FileSourceFolder();
            fileSourceFolder.Path = test_path;

            // Add the new source folder to the DB and save.
            fileSourceFolderRepository.Add(fileSourceFolder);
            fileSourceFolderRepository.Save();

            id = fileSourceFolder.FileSourceFolderID;


            FileSourceFolder verifyFileSourceFolder;

            // Verify that the new source folder was saved.
            verifyFileSourceFolder = fileSourceFolderRepository.Get(id);

            // Found
            Assert.NotNull(verifyFileSourceFolder);
            // Same path
            Assert.AreEqual(verifyFileSourceFolder.Path, test_path);
            // No exclude paths
            Assert.IsEmpty(verifyFileSourceFolder.ExcludePaths);

        }



        [Test]
        public void CreateAndSaveFileSourceFolderWithTwoExcludePathsSavesToDB()
        {
            int id;
            string test_path = @"\Create\And\Save\FileSourceFolder\With\Two\Exclude\Paths\Saves\To\DB";
            string test_exclude_path1 = @"\Create\And\Save\FileSourceFolder\With\Two\Exclude\Paths\Saves\To\DB\Exclude1";
            string test_exclude_path2 = @"\Create\And\Save\FileSourceFolder\With\Two\Exclude\Paths\Saves\To\DB\Exclude2";

            FileSourceFolder fileSourceFolder = new FileSourceFolder();

            fileSourceFolder.Path = test_path;

            var excludePath1 = new ExcludePath();
            excludePath1.Path = test_exclude_path1;

            var excludePath2 = new ExcludePath();
            excludePath2.Path = test_exclude_path2;

            fileSourceFolder.ExcludePaths.Add(excludePath1);
            fileSourceFolder.ExcludePaths.Add(excludePath2);

            // Add the new source folder to the DB and save.
            fileSourceFolderRepository.Add(fileSourceFolder);
            fileSourceFolderRepository.Save();

            id = fileSourceFolder.FileSourceFolderID;

            FileSourceFolder verifyFileSourceFolder;

            // Verify that the new source folder was saved.
            verifyFileSourceFolder = fileSourceFolderRepository.Get(id);

            // Found
            Assert.NotNull(verifyFileSourceFolder);
            // Same path
            Assert.AreEqual(test_path, verifyFileSourceFolder.Path);
            // Two exclude paths
            Assert.AreEqual(2, verifyFileSourceFolder.ExcludePaths.Count);
            // One path is the first exclude path
            Assert.IsTrue(verifyFileSourceFolder.ExcludePaths.Any(p => p.Path == test_exclude_path1));
            // One path is the second exclude path
            Assert.IsTrue(verifyFileSourceFolder.ExcludePaths.Any(p => p.Path == test_exclude_path2));
        }

        [Test]
        public void UpdateAndSaveFileSourceFolderWithTwoExcludePathsToThreeExcludePathsSavesToDB()
        {
            int id;
            string test_path = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB";
            string test_exclude_path1 = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB\Exclude1";
            string test_exclude_path2 = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB\Exclude2";
            string test_exclude_path3 = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB\Exclude3";

            FileSourceFolder fileSourceFolder = new FileSourceFolder();

            // Add the new source folder to the DB and save.
            fileSourceFolder.Path = test_path;

            var excludePath1 = new ExcludePath();
            excludePath1.Path = test_exclude_path1;

            var excludePath2 = new ExcludePath();
            excludePath2.Path = test_exclude_path2;

            fileSourceFolder.ExcludePaths.Add(excludePath1);
            fileSourceFolder.ExcludePaths.Add(excludePath2);

            fileSourceFolderRepository.Add(fileSourceFolder);
            fileSourceFolderRepository.Save();


            id = fileSourceFolder.FileSourceFolderID;


            FileSourceFolder updateFileSourceFolder;

            // Update the source folder just added to have three exclude paths
            updateFileSourceFolder = fileSourceFolderRepository.Get(id);

            var excludePath3 = new ExcludePath();
            excludePath3.Path = test_exclude_path3;

            updateFileSourceFolder.ExcludePaths.Add(excludePath3);

            fileSourceFolderRepository.Save();


            FileSourceFolder verifyFileSourceFolder;

            verifyFileSourceFolder = fileSourceFolderRepository.Get(id);


            // Found
            Assert.NotNull(verifyFileSourceFolder);
            // Same path
            Assert.AreEqual(test_path, verifyFileSourceFolder.Path);
            // Two exclude paths
            Assert.AreEqual(3, verifyFileSourceFolder.ExcludePaths.Count);
            // One path is the first exclude path
            Assert.IsTrue(verifyFileSourceFolder.ExcludePaths.Any(p => p.Path == test_exclude_path1));
            // One path is the second exclude path
            Assert.IsTrue(verifyFileSourceFolder.ExcludePaths.Any(p => p.Path == test_exclude_path2));
            // One path is the third exclude path
            Assert.IsTrue(verifyFileSourceFolder.ExcludePaths.Any(p => p.Path == test_exclude_path3));
        }


        [Test]
        public void UpdateAndSaveFileSourceFolderWithThreeExcludePathsToTwoExcludePathsSavesToDB()
        {
            int id;
            string test_path = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB";
            string test_exclude_path1 = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB\Exclude1";
            string test_exclude_path2 = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB\Exclude2";
            string test_exclude_path3 = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB\Exclude3";

            FileSourceFolder fileSourceFolder = new FileSourceFolder();

            // Add the new source folder to the DB and save.
            fileSourceFolder.Path = test_path;

            var excludePath1 = new ExcludePath();
            excludePath1.Path = test_exclude_path1;

            var excludePath2 = new ExcludePath();
            excludePath2.Path = test_exclude_path2;

            var excludePath3 = new ExcludePath();
            excludePath3.Path = test_exclude_path3;

            fileSourceFolder.ExcludePaths.Add(excludePath1);
            fileSourceFolder.ExcludePaths.Add(excludePath2);
            fileSourceFolder.ExcludePaths.Add(excludePath3);

            fileSourceFolderRepository.Add(fileSourceFolder);
            fileSourceFolderRepository.Save();

            id = fileSourceFolder.FileSourceFolderID;


            FileSourceFolder updateFileSourceFolder;

            // Update the source folder just added to have only two exclude paths
            updateFileSourceFolder = fileSourceFolderRepository.Get(id);

            var excludePathToRemove = updateFileSourceFolder.ExcludePaths.First();

            updateFileSourceFolder.ExcludePaths.Remove(excludePathToRemove);

            fileSourceFolderRepository.Save();


            FileSourceFolder verifyFileSourceFolder;

            verifyFileSourceFolder = fileSourceFolderRepository.Get(id);


            // Found
            Assert.NotNull(verifyFileSourceFolder);
            // Same path
            Assert.AreEqual(test_path, verifyFileSourceFolder.Path);
            // Two exclude paths
            Assert.AreEqual(2, verifyFileSourceFolder.ExcludePaths.Count);

        }


        [Test]
        public void FileSourceFolderWithTwoExcludePathsDeletesFromDB()
        {
            int id;
            string test_path = @"\FileSourceFolder\With\Two\ExcludePaths\Deletes\From\DB";
            string test_exclude_path1 = @"\FileSourceFolder\With\Two\ExcludePaths\Deletes\From\DB\Exclude1";
            string test_exclude_path2 = @"\FileSourceFolder\With\Two\ExcludePaths\Deletes\From\DB\Exclude2";

            string exclude_path_count_sql = "SELECT COUNT(*) FROM ExcludePaths WHERE FileSourceFolderID = @id";

            FileSourceFolder fileSourceFolder = new FileSourceFolder();

            // Add the new source folder to the DB and save.
            fileSourceFolder.Path = test_path;

            var excludePath1 = new ExcludePath();
            excludePath1.Path = test_exclude_path1;

            var excludePath2 = new ExcludePath();
            excludePath2.Path = test_exclude_path2;

            fileSourceFolder.ExcludePaths.Add(excludePath1);
            fileSourceFolder.ExcludePaths.Add(excludePath2);

            fileSourceFolderRepository.Add(fileSourceFolder);
            fileSourceFolderRepository.Save();

            id = fileSourceFolder.FileSourceFolderID;


            // Verify that the excludepaths exist with the query we'll use for checking that they got removed.
            using (var datacontext = new DataContext())
            {
                int exclude_paths_count = datacontext.Database.SqlQuery<int>(exclude_path_count_sql, new SqlParameter("@id", id)).FirstOrDefault();

                // Should have two exclude paths for that id.
                Assert.AreEqual(2, exclude_paths_count);
            }

            FileSourceFolder deleteFileSourceFolder;

            // Delete the source folder from the DB and save.
            deleteFileSourceFolder = fileSourceFolderRepository.Get(id);
            fileSourceFolderRepository.Remove(deleteFileSourceFolder);
            fileSourceFolderRepository.Save();


            FileSourceFolder verifyFileSourceFolder;

            // Verify that the source folder was removed.
            verifyFileSourceFolder = fileSourceFolderRepository.Get(id);

            Assert.IsNull(verifyFileSourceFolder);


            // Verify that the excludepaths it contained were also removed.
            using (var datacontext = new DataContext())
            {
                int exclude_paths_count = datacontext.Database.SqlQuery<int>(exclude_path_count_sql, new SqlParameter("@id", id)).FirstOrDefault();

                // Should have no exclude paths for that id.
                Assert.AreEqual(0, exclude_paths_count);
            }
        }


        [Test]
        public void GetAllReturnsExistingFileSourceFoldersIncludingExcludePaths()
        {
            int id1, id2;
            string test_path1 = @"\GetAll\Returns\Existing\FileSourceFolders\Including\ExcludePaths1";
            string test_path2 = @"\GetAll\Returns\Existing\FileSourceFolders\Including\ExcludePaths2";
            string test_exclude_path1 = test_path2 + @"\Exclude1";
            string test_exclude_path2 = test_path2 + @"\Exclude2";

            FileSourceFolder fileSourceFolder1 = new FileSourceFolder();
            FileSourceFolder fileSourceFolder2 = new FileSourceFolder();

            // Add the two source folders to the DB and save.
            fileSourceFolder1.Path = test_path1;

            fileSourceFolder2.Path = test_path2;

            var excludePath1 = new ExcludePath();
            excludePath1.Path = test_exclude_path1;

            var excludePath2 = new ExcludePath();
            excludePath2.Path = test_exclude_path2;

            fileSourceFolder2.ExcludePaths.Add(excludePath1);
            fileSourceFolder2.ExcludePaths.Add(excludePath2);

            fileSourceFolderRepository.Add(fileSourceFolder1);
            fileSourceFolderRepository.Add(fileSourceFolder2);
            fileSourceFolderRepository.Save();

            id1 = fileSourceFolder1.FileSourceFolderID;
            id2 = fileSourceFolder2.FileSourceFolderID;



            FileSourceFolder verifyFileSourceFolder1;
            FileSourceFolder verifyFileSourceFolder2;

            // Verify that GetAll retrieves both source folders and that the second source folder has exclude paths.
            var fileSourceFolders = fileSourceFolderRepository.GetAll();

            // Found two folders
            Assert.AreEqual(2, fileSourceFolders.Count);

            verifyFileSourceFolder1 = fileSourceFolders.Find(f => f.FileSourceFolderID == id1);
            verifyFileSourceFolder2 = fileSourceFolders.Find(f => f.FileSourceFolderID == id2);

            // We found the first folder
            Assert.NotNull(verifyFileSourceFolder1);
            //We found the second folder
            Assert.NotNull(verifyFileSourceFolder2);

            // Verify the first folder exclude paths (i.e., none)
            Assert.AreEqual(0, verifyFileSourceFolder1.ExcludePaths.Count);

            // Verify the second folder exclude paths (two exclude paths)
            // Two exclude paths
            Assert.AreEqual(2, verifyFileSourceFolder2.ExcludePaths.Count);
            // One path is the first exclude path
            Assert.IsTrue(verifyFileSourceFolder2.ExcludePaths.Any(p => p.Path == test_exclude_path1));
            // One path is the second exclude path
            Assert.IsTrue(verifyFileSourceFolder2.ExcludePaths.Any(p => p.Path == test_exclude_path2));

        }

        
    }
}
