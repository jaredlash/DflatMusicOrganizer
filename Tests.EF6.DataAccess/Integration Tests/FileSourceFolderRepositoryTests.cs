using Dflat.Business;
using Dflat.Business.Models;
using Dflat.EF6.DataAccess;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Linq;

namespace Tests.EF6.DataAccess.Integration_Tests
{
    [TestFixture]
    public class FileSourceFolderRepositoryTests : EFDataTest
    {
        
        [SetUp]
        public override void TestInitialize()
        {
            base.TestInitialize();

            IUnitOfWork unitOfWork = new UnitOfWork();

        }

        public FileSourceFolderRepositoryTests()
        {

        }


        [Test]
        public void CreateFromFileSourceFolderRepository()
        {
            IUnitOfWork unitOfWork;

            using (unitOfWork = new UnitOfWork())
            {
                var fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Create();
                Assert.IsNotNull(fileSourceFolder);
                Assert.IsNotNull(fileSourceFolder.ExcludePaths);
            }
        }


        [Test]
        public void CreateAndSaveFileSourceFolderSetsID()
        {
            IUnitOfWork unitOfWork;
            int saved_id = 0;
            string test_path = @"\Create\And\Save\FileSourceFolder\Sets\ID";

            // Add the new source folder to the DB and save.
            using (unitOfWork = new UnitOfWork())
            {
                IFileSourceFolder fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Create();
                fileSourceFolder.Path = test_path;

                unitOfWork.SaveChanges();
                saved_id = fileSourceFolder.FileSourceFolderID;
            }

            Assert.AreNotEqual(saved_id, 0);
            
        }

        [Test]
        public void CreateAndSaveFileSourceFolderWithNoExcludePathsSavesToDB()
        {
            IUnitOfWork unitOfWork;
            int saved_id;
            string test_path = @"\Create\And\Save\FileSourceFolder\With\No\Exclude\Paths\Saves\To\DB";

            // Add the new source folder to the DB and save.
            using (unitOfWork = new UnitOfWork())
            {
                IFileSourceFolder fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Create();
                fileSourceFolder.Path = test_path;

                unitOfWork.SaveChanges();
                saved_id = fileSourceFolder.FileSourceFolderID;
            }

            // Verify that the new source folder was saved.
            using (unitOfWork = new UnitOfWork())
            {
                IFileSourceFolder fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Get(saved_id);

                // Found
                Assert.NotNull(fileSourceFolder);
                // Same path
                Assert.AreEqual(fileSourceFolder.Path, test_path);
                // No exclude paths
                Assert.IsEmpty(fileSourceFolder.ExcludePaths);
            }
        }



        [Test]
        public void CreateFileSourceFolderHasChangesToSave()
        {
            IUnitOfWork unitOfWork;
            string test_path = @"\Create\FileSourceFolder\Has\Changes\To\Save";

            // Add the new source folder to the DB and save.
            using (unitOfWork = new UnitOfWork())
            {
                IFileSourceFolder fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Create();
                fileSourceFolder.Path = test_path;

                Assert.IsTrue(unitOfWork.HasChanges());
            }
        }


        [Test]
        public void CreateAndSaveFileSourceFolderWithTwoExcludePathsSavesToDB()
        {
            IUnitOfWork unitOfWork;
            int saved_id;
            string test_path = @"\Create\And\Save\FileSourceFolder\With\Two\Exclude\Paths\Saves\To\DB";
            string test_exclude_path1 = @"\Create\And\Save\FileSourceFolder\With\Two\Exclude\Paths\Saves\To\DB\Exclude1";
            string test_exclude_path2 = @"\Create\And\Save\FileSourceFolder\With\Two\Exclude\Paths\Saves\To\DB\Exclude2";

            // Add the new source folder to the DB and save.
            using (unitOfWork = new UnitOfWork())
            {
                IFileSourceFolder fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Create();
                fileSourceFolder.Path = test_path;

                var excludePath1 = new ExcludePath();
                excludePath1.Path = test_exclude_path1;

                var excludePath2 = new ExcludePath();
                excludePath2.Path = test_exclude_path2;

                fileSourceFolder.ExcludePaths.Add(excludePath1);
                fileSourceFolder.ExcludePaths.Add(excludePath2);

                unitOfWork.SaveChanges();
                saved_id = fileSourceFolder.FileSourceFolderID;
            }

            // Verify that the new source folder was saved.
            using (unitOfWork = new UnitOfWork())
            {
                IFileSourceFolder fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Get(saved_id);

                // Found
                Assert.NotNull(fileSourceFolder);
                // Same path
                Assert.AreEqual(test_path, fileSourceFolder.Path);
                // Two exclude paths
                Assert.AreEqual(2, fileSourceFolder.ExcludePaths.Count);
                // One path is the first exclude path
                Assert.IsTrue(fileSourceFolder.ExcludePaths.Any(p => p.Path == test_exclude_path1));
                // One path is the second exclude path
                Assert.IsTrue(fileSourceFolder.ExcludePaths.Any(p => p.Path == test_exclude_path2));
            }
        }

        [Test]
        public void UpdateAndSaveFileSourceFolderWithTwoExcludePathsToThreeExcludePathsSavesToDB()
        {
            IUnitOfWork unitOfWork;
            int saved_id;
            string test_path = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB";
            string test_exclude_path1 = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB\Exclude1";
            string test_exclude_path2 = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB\Exclude2";
            string test_exclude_path3 = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB\Exclude3";

            // Add the new source folder to the DB and save.
            using (unitOfWork = new UnitOfWork())
            {
                IFileSourceFolder fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Create();
                fileSourceFolder.Path = test_path;

                var excludePath1 = new ExcludePath();
                excludePath1.Path = test_exclude_path1;

                var excludePath2 = new ExcludePath();
                excludePath2.Path = test_exclude_path2;

                fileSourceFolder.ExcludePaths.Add(excludePath1);
                fileSourceFolder.ExcludePaths.Add(excludePath2);

                unitOfWork.SaveChanges();
                saved_id = fileSourceFolder.FileSourceFolderID;
            }

            // Update the source folder just added to have three exclude paths
            using (unitOfWork = new UnitOfWork())
            {
                IFileSourceFolder fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Get(saved_id);

                var excludePath3 = new ExcludePath();
                excludePath3.Path = test_exclude_path3;

                fileSourceFolder.ExcludePaths.Add(excludePath3);

                unitOfWork.SaveChanges();
            }

            using (unitOfWork = new UnitOfWork())
            {
                IFileSourceFolder fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Get(saved_id);
            

                // Found
                Assert.NotNull(fileSourceFolder);
                // Same path
                Assert.AreEqual(test_path, fileSourceFolder.Path);
                // Two exclude paths
                Assert.AreEqual(3, fileSourceFolder.ExcludePaths.Count);
                // One path is the first exclude path
                Assert.IsTrue(fileSourceFolder.ExcludePaths.Any(p => p.Path == test_exclude_path1));
                // One path is the second exclude path
                Assert.IsTrue(fileSourceFolder.ExcludePaths.Any(p => p.Path == test_exclude_path2));
                // One path is the third exclude path
                Assert.IsTrue(fileSourceFolder.ExcludePaths.Any(p => p.Path == test_exclude_path3));
            }
        }


        [Test]
        public void UpdateAndSaveFileSourceFolderWithThreeExcludePathsToTwoExcludePathsSavesToDB()
        {
            IUnitOfWork unitOfWork;
            int saved_id;
            string test_path = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB";
            string test_exclude_path1 = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB\Exclude1";
            string test_exclude_path2 = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB\Exclude2";
            string test_exclude_path3 = @"\Update\And\Save\FileSourceFolder\With\Two\ExcludePaths\To\Three\ExcludePaths\Saves\To\DB\Exclude3";

            // Add the new source folder to the DB and save.
            using (unitOfWork = new UnitOfWork())
            {
                IFileSourceFolder fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Create();
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

                unitOfWork.SaveChanges();
                saved_id = fileSourceFolder.FileSourceFolderID;
            }

            // Update the source folder just added to have only two exclude paths
            using (unitOfWork = new UnitOfWork())
            {
                IFileSourceFolder fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Get(saved_id);

                var excludePathToRemove = fileSourceFolder.ExcludePaths.First();

                fileSourceFolder.ExcludePaths.Remove(excludePathToRemove);

                unitOfWork.SaveChanges();
            }

            using (unitOfWork = new UnitOfWork())
            {
                IFileSourceFolder fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Get(saved_id);


                // Found
                Assert.NotNull(fileSourceFolder);
                // Same path
                Assert.AreEqual(test_path, fileSourceFolder.Path);
                // Two exclude paths
                Assert.AreEqual(2, fileSourceFolder.ExcludePaths.Count);
            }
        }


        [Test]
        public void FileSourceFolderWithTwoExcludePathsDeletesFromDB()
        {
            IUnitOfWork unitOfWork;
            int saved_id;
            string test_path = @"\FileSourceFolder\With\Two\ExcludePaths\Deletes\From\DB";
            string test_exclude_path1 = @"\FileSourceFolder\With\Two\ExcludePaths\Deletes\From\DB\Exclude1";
            string test_exclude_path2 = @"\FileSourceFolder\With\Two\ExcludePaths\Deletes\From\DB\Exclude2";

            string exclude_path_count_sql = "SELECT COUNT(*) FROM ExcludePaths WHERE FileSourceFolderID = @id";

            // Add the new source folder to the DB and save.
            using (unitOfWork = new UnitOfWork())
            {
                var fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Create();
                fileSourceFolder.Path = test_path;

                var excludePath1 = new ExcludePath();
                excludePath1.Path = test_exclude_path1;

                var excludePath2 = new ExcludePath();
                excludePath2.Path = test_exclude_path2;

                fileSourceFolder.ExcludePaths.Add(excludePath1);
                fileSourceFolder.ExcludePaths.Add(excludePath2);

                unitOfWork.SaveChanges();
                saved_id = fileSourceFolder.FileSourceFolderID;
            }

            // Verify that the excludepaths exist with the query we'll use for checking that they got removed.
            using (var datacontext = new DataContext())
            {
                int exclude_paths_count = datacontext.Database.SqlQuery<int>(exclude_path_count_sql, new SqlParameter("@id", saved_id)).FirstOrDefault();

                // Should have two exclude paths for that id.
                Assert.AreEqual(2, exclude_paths_count);
            }

            // Delete the source folder from the DB and save.
            using (unitOfWork = new UnitOfWork())
            {
                var fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Get(saved_id);

                unitOfWork.IFileSourceFolderRepository.Remove(fileSourceFolder);

                unitOfWork.SaveChanges();
            }


            // Verify that the source folder was removed.
            using (unitOfWork = new UnitOfWork())
            {
                var fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Get(saved_id);

                Assert.IsNull(fileSourceFolder);
            }

            // Verify that the excludepaths it contained were also removed.
            using (var datacontext = new DataContext())
            {
                int exclude_paths_count = datacontext.Database.SqlQuery<int>(exclude_path_count_sql, new SqlParameter("@id", saved_id)).FirstOrDefault();

                // Should have no exclude paths for that id.
                Assert.AreEqual(0, exclude_paths_count);
            }
        }


        [Test]
        public void GetAllReturnsExistingFileSourceFoldersIncludingExcludePaths()
        {
            IUnitOfWork unitOfWork;
            int saved_id1, saved_id2;
            string test_path1 = @"\GetAll\Returns\Existing\FileSourceFolders\Including\ExcludePaths1";
            string test_path2 = @"\GetAll\Returns\Existing\FileSourceFolders\Including\ExcludePaths2";
            string test_exclude_path1 = test_path2 + @"\Exclude1";
            string test_exclude_path2 = test_path2 + @"\Exclude2";

            // Add the two source folders to the DB and save.
            using (unitOfWork = new UnitOfWork())
            {
                var fileSourceFolder1 = unitOfWork.IFileSourceFolderRepository.Create();
                fileSourceFolder1.Path = test_path1;

                var fileSourceFolder2 = unitOfWork.IFileSourceFolderRepository.Create();
                fileSourceFolder2.Path = test_path2;

                var excludePath1 = new ExcludePath();
                excludePath1.Path = test_exclude_path1;

                var excludePath2 = new ExcludePath();
                excludePath2.Path = test_exclude_path2;

                fileSourceFolder2.ExcludePaths.Add(excludePath1);
                fileSourceFolder2.ExcludePaths.Add(excludePath2);

                unitOfWork.SaveChanges();
                saved_id1 = fileSourceFolder1.FileSourceFolderID;
                saved_id2 = fileSourceFolder2.FileSourceFolderID;
            }

            // Verify that GetAll retrieves both source folders and that the second source folder has exclude paths.
            using (unitOfWork = new UnitOfWork())
            {
                var fileSourceFolders = unitOfWork.IFileSourceFolderRepository.GetAll();

                // Found two folders
                Assert.AreEqual(2, fileSourceFolders.Count);

                var fileSourceFolder1 = fileSourceFolders.Find(f => f.FileSourceFolderID == saved_id1);
                var fileSourceFolder2 = fileSourceFolders.Find(f => f.FileSourceFolderID == saved_id2);

                // We found the first folder
                Assert.NotNull(fileSourceFolder1);
                //We found the second folder
                Assert.NotNull(fileSourceFolder2);

                // Verify the first folder exclude paths (i.e., none)
                Assert.AreEqual(0, fileSourceFolder1.ExcludePaths.Count);

                // Verify the second folder exclude paths (two exclude paths)
                // Two exclude paths
                Assert.AreEqual(2, fileSourceFolder2.ExcludePaths.Count);
                // One path is the first exclude path
                Assert.IsTrue(fileSourceFolder2.ExcludePaths.Any(p => p.Path == test_exclude_path1));
                // One path is the second exclude path
                Assert.IsTrue(fileSourceFolder2.ExcludePaths.Any(p => p.Path == test_exclude_path2));
                
            }
        }

        [Test]
        public void FileSourceFolderWithTwoExcludePathsDeletesFromDBUsingIDNotTracked()
        {
            IUnitOfWork unitOfWork;
            DataContext datacontext;
            int saved_id;
            string test_path = @"\FileSourceFolder\With\Two\ExcludePaths\Deletes\From\DB\Using\ID\Not\Tracked";
            string test_exclude_path1 = test_path + @"\Exclude1";
            string test_exclude_path2 = test_path + @"\Exclude2";

            string exclude_path_count_sql = "SELECT COUNT(*) FROM ExcludePaths WHERE FileSourceFolderID = @id";

            // Add the new source folder to the DB and save.
            using (unitOfWork = new UnitOfWork())
            {
                var fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Create();
                fileSourceFolder.Path = test_path;

                var excludePath1 = new ExcludePath();
                excludePath1.Path = test_exclude_path1;

                var excludePath2 = new ExcludePath();
                excludePath2.Path = test_exclude_path2;

                fileSourceFolder.ExcludePaths.Add(excludePath1);
                fileSourceFolder.ExcludePaths.Add(excludePath2);

                unitOfWork.SaveChanges();
                saved_id = fileSourceFolder.FileSourceFolderID;
            }

            // Verify that the excludepaths exist with the query we'll use for checking that they got removed.
            using (datacontext = new DataContext())
            {
                int exclude_paths_count = datacontext.Database.SqlQuery<int>(exclude_path_count_sql, new SqlParameter("@id", saved_id)).FirstOrDefault();

                // Should have two exclude paths for that id.
                Assert.AreEqual(2, exclude_paths_count);
            }

            // Delete the source folder from the DB and save.
            datacontext = new DataContext();
            using (unitOfWork = new UnitOfWork(datacontext))
            {
                var tracked = datacontext.ChangeTracker.Entries<FileSourceFolder>().Any(p => p.Entity.FileSourceFolderID == saved_id);

                //Verify that we're not tracking this folder
                Assert.IsFalse(tracked);

                unitOfWork.IFileSourceFolderRepository.Remove(saved_id);

                unitOfWork.SaveChanges();
            }


            // Verify that the source folder was removed.
            using (unitOfWork = new UnitOfWork())
            {
                var fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Get(saved_id);

                Assert.IsNull(fileSourceFolder);
            }

            // Verify that the excludepaths it contained were also removed.
            using (datacontext = new DataContext())
            {
                int exclude_paths_count = datacontext.Database.SqlQuery<int>(exclude_path_count_sql, new SqlParameter("@id", saved_id)).FirstOrDefault();

                // Should have no exclude paths for that id.
                Assert.AreEqual(0, exclude_paths_count);
            }
        }

        [Test]
        public void FileSourceFolderWithTwoExcludePathsDeletesFromDBUsingIDTracked()
        {
            IUnitOfWork unitOfWork;
            DataContext datacontext;
            int saved_id;
            string test_path = @"\FileSourceFolder\With\Two\ExcludePaths\Deletes\From\DB\Using\ID\Tracked";
            string test_exclude_path1 = test_path + @"\Exclude1";
            string test_exclude_path2 = test_path + @"\Exclude2";

            string exclude_path_count_sql = "SELECT COUNT(*) FROM ExcludePaths WHERE FileSourceFolderID = @id";

            // Add the new source folder to the DB and save.
            using (unitOfWork = new UnitOfWork())
            {
                var fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Create();
                fileSourceFolder.Path = test_path;

                var excludePath1 = new ExcludePath();
                excludePath1.Path = test_exclude_path1;

                var excludePath2 = new ExcludePath();
                excludePath2.Path = test_exclude_path2;

                fileSourceFolder.ExcludePaths.Add(excludePath1);
                fileSourceFolder.ExcludePaths.Add(excludePath2);

                unitOfWork.SaveChanges();
                saved_id = fileSourceFolder.FileSourceFolderID;
            }

            // Verify that the excludepaths exist with the query we'll use for checking that they got removed.
            using (datacontext = new DataContext())
            {
                int exclude_paths_count = datacontext.Database.SqlQuery<int>(exclude_path_count_sql, new SqlParameter("@id", saved_id)).FirstOrDefault();

                // Should have two exclude paths for that id.
                Assert.AreEqual(2, exclude_paths_count);
            }

            // Delete the source folder from the DB and save.
            datacontext = new DataContext();
            using (unitOfWork = new UnitOfWork(datacontext))
            {
                var load_folder_to_track = unitOfWork.IFileSourceFolderRepository.Get(saved_id);

                var tracked = datacontext.ChangeTracker.Entries<FileSourceFolder>().Any(p => p.Entity.FileSourceFolderID == saved_id);

                //Verify that we ARE tracking this folder
                Assert.IsTrue(tracked);

                unitOfWork.IFileSourceFolderRepository.Remove(saved_id);

                unitOfWork.SaveChanges();
            }


            // Verify that the source folder was removed.
            using (unitOfWork = new UnitOfWork())
            {
                var fileSourceFolder = unitOfWork.IFileSourceFolderRepository.Get(saved_id);

                Assert.IsNull(fileSourceFolder);
            }

            // Verify that the excludepaths it contained were also removed.
            using (datacontext = new DataContext())
            {
                int exclude_paths_count = datacontext.Database.SqlQuery<int>(exclude_path_count_sql, new SqlParameter("@id", saved_id)).FirstOrDefault();

                // Should have no exclude paths for that id.
                Assert.AreEqual(0, exclude_paths_count);
            }
        }
    }
}
