using Dapper;
using Dapper.Transaction;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Dflat.Data.Dapper.Repositories
{
    public class FileSourceFolderRepository : IFileSourceFolderRepository
    {
        private readonly string connectionString;

        public FileSourceFolderRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public FileSourceFolder Get(int fileSourceFolderID)
        {
            const string sql = @"SELECT f.FileSourceFolderID
                                       ,f.Name
                                       ,f.Path
                                       ,f.IsTemporaryMedia
                                       ,f.LastScanStart
                                 	   ,e.ExcludePathID
                                       ,e.Path
                                       ,e.FileSourceFolderID
                                   FROM dbo.FileSourceFolders f
                        LEFT OUTER JOIN dbo.ExcludePaths e
                                     ON e.FileSourceFolderID = f.FileSourceFolderID
                                  WHERE f.FileSourceFolderID = @FileSourceFolderID;";

            using IDbConnection conn = new SqlConnection(connectionString);

            var lookup = new Dictionary<int, FileSourceFolder>();
            var results = conn.Query<FileSourceFolder, ExcludePath, FileSourceFolder>(sql, (f, e) =>
            {
                if (!lookup.TryGetValue(f.FileSourceFolderID, out FileSourceFolder fileSourceFolder))
                {
                    lookup.Add(f.FileSourceFolderID, f);
                    fileSourceFolder = f;
                }

                if (e != null)
                    fileSourceFolder.ExcludePaths.Add(e);

                return fileSourceFolder;
            }, splitOn: "ExcludePathID", param: new { FileSourceFolderID = fileSourceFolderID });

            int count = results.Count();
            if (count == 0)
                return null;

            var result = results.FirstOrDefault();
            if (result != null)
                result.IsChanged = false;


            return result;
        }

        public IEnumerable<FileSourceFolder> GetAll()
        {
            const string sql = @"SELECT f.FileSourceFolderID
                                       ,f.Name
                                       ,f.Path
                                       ,f.IsTemporaryMedia
                                       ,f.LastScanStart
                                 	   ,e.ExcludePathID
                                       ,e.Path
                                       ,e.FileSourceFolderID
                                   FROM dbo.FileSourceFolders f
                        LEFT OUTER JOIN dbo.ExcludePaths e
                                     ON e.FileSourceFolderID = f.FileSourceFolderID;";

            using IDbConnection conn = new SqlConnection(connectionString);

            var lookup = new Dictionary<int, FileSourceFolder>();
            var results = conn.Query<FileSourceFolder, ExcludePath, FileSourceFolder>(sql, (f, e) =>
            {
                if (!lookup.TryGetValue(f.FileSourceFolderID, out FileSourceFolder fileSourceFolder))
                {
                    lookup.Add(f.FileSourceFolderID, f);
                    fileSourceFolder = f;
                }

                if (e != null)
                    fileSourceFolder.ExcludePaths.Add(e);

                return fileSourceFolder;
            }, splitOn: "ExcludePathID").Distinct();

            foreach (var result in results)
                result.IsChanged = false;


            return results;
        }

        public Task<IEnumerable<FileSourceFolder>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAllAsync(IEnumerable<FileSourceFolder> fileSourceFolders)
        {
            using IDbConnection connection = new SqlConnection(connectionString);

            connection.Open();

            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    // Remove any deleted fileSourceFolders and their exclude paths
                    await RemoveSourceFoldersNotInListAsync(transaction, fileSourceFolders);

                    // Handle the FileSourceFolders we are keeping
                    foreach (var fileSourceFolder in fileSourceFolders)
                    {

                        // Add new folders (ID == 0)
                        if (fileSourceFolder.FileSourceFolderID == 0)
                        {
                            await AddFileSourceFolderAsync(transaction, fileSourceFolder);
                        }
                        else // Otherwise, update folder
                        {
                            await UpdateFileSourceFolderAsync(transaction, fileSourceFolder);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            // clear IsChanged flag to indicate object matches DB
            foreach (var fileSourceFolder in fileSourceFolders)
                fileSourceFolder.IsChanged = false;

            return true;
        }

        public async Task AddOrUpdateAsync(FileSourceFolder fileSourceFolder)
        {
            using IDbConnection connection = new SqlConnection(connectionString);

            connection.Open();

            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    // Add new folders (ID == 0)
                    if (fileSourceFolder.FileSourceFolderID == 0)
                    {
                        await AddFileSourceFolderAsync(transaction, fileSourceFolder);
                    }
                    else // Otherwise, update folder
                    {
                        await UpdateFileSourceFolderAsync(transaction, fileSourceFolder);
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            fileSourceFolder.IsChanged = false;
        }

        public Task UpdateLastScanTimeAsync(int fileSourceFolderID)
        {
            const string updateFileSourceFolderSql = @"UPDATE [dbo].[FileSourceFolders]
                                                          SET [LastScanStart] = @LastScanStart
                                                        WHERE FileSourceFolderID = @FileSourceFolderID";
            using IDbConnection connection = new SqlConnection(connectionString);
            
            return connection.ExecuteAsync(updateFileSourceFolderSql, new { LastScanStart = DateTime.Now, FileSourceFolderID = fileSourceFolderID });
        }


        #region Private methods

        // Removes any FileSourceFolders and their exclude paths that are not found in the provided set
        private async Task RemoveSourceFoldersNotInListAsync(IDbTransaction transaction, IEnumerable<FileSourceFolder> fileSourceFoldersToKeep)
        {
            HashSet<int> foldersToRemove = new HashSet<int>();

            const string allFolderIDsSql = @"SELECT [FileSourceFolderID] FROM [dbo].[FileSourceFolders]";
            const string removeExcludePathsSql = @"DELETE FROM ExcludePaths WHERE FileSourceFolderID = @FileSourceFolderID";
            const string removeFileSourceFolderSql = @"DELETE FROM FileSourceFolders WHERE FileSourceFolderID = @FileSourceFolderID";

            // Get our current folder IDs in the database
            var results = await transaction.QueryAsync<int>(allFolderIDsSql);
            foreach (int id in results)
                foldersToRemove.Add(id);

            // Remove the IDs we want to keep so we are left with the folders to remove
            foreach (var folder in fileSourceFoldersToKeep)
                foldersToRemove.Remove(folder.FileSourceFolderID);

            // Remove the associated Folders and ExcludePaths
            foreach (int fileSourceFolderID in foldersToRemove)
            {
                await transaction.ExecuteAsync(removeExcludePathsSql, new { FileSourceFolderID = fileSourceFolderID });
                await transaction.ExecuteAsync(removeFileSourceFolderSql, new { FileSourceFolderID = fileSourceFolderID });
            }
        }


        private async Task AddFileSourceFolderAsync(IDbTransaction transaction, FileSourceFolder fileSourceFolder)
        {
            const string addFileSourceFolderSql = @"INSERT INTO [dbo].[FileSourceFolders]
                                                               ([Name]
                                                               ,[Path]
                                                               ,[IsTemporaryMedia]
                                                               ,[LastScanStart])
                                                         VALUES
                                                               (@Name
                                                               ,@Path
                                                               ,@IsTemporaryMedia
                                                               ,@LastScanStart);
                                                    SELECT CAST(SCOPE_IDENTITY() as int);";

            var model = new DynamicParameters(fileSourceFolder)
            {
                RemoveUnused = true
            };

            fileSourceFolder.FileSourceFolderID = await transaction.QuerySingleAsync<int>(addFileSourceFolderSql, model);

            foreach (var excludePath in fileSourceFolder.ExcludePaths)
            {
                await AddExcludePathAsync(transaction, fileSourceFolder.FileSourceFolderID, excludePath);
            }
        }

        private async Task AddExcludePathAsync(IDbTransaction transaction, int fileSourceFolderID, ExcludePath excludePath)
        {
            const string addExcludePathSql = @"INSERT INTO [dbo].[ExcludePaths]
                                                          ([Path]
                                                          ,[FileSourceFolderID])
                                                    VALUES
                                                          (@Path
                                                          ,@FileSourceFolderID);
                                              SELECT CAST(SCOPE_IDENTITY() as int);";

            var model = new DynamicParameters(excludePath);
            model.Add("@FileSourceFolderID", fileSourceFolderID);
            model.RemoveUnused = true;

            excludePath.ExcludePathID = await transaction.QuerySingleAsync<int>(addExcludePathSql, model);
        }


        private async Task UpdateFileSourceFolderAsync(IDbTransaction transaction, FileSourceFolder fileSourceFolder)
        {
            const string updateFileSourceFolderSql = @"UPDATE [dbo].[FileSourceFolders]
                                                          SET [Name] = @Name
                                                             ,[Path] = @Path
                                                             ,[IsTemporaryMedia] = @IsTemporaryMedia
                                                             ,[LastScanStart] = @LastScanStart
                                                        WHERE FileSourceFolderID = @FileSourceFolderID";
            await transaction.ExecuteAsync(updateFileSourceFolderSql, fileSourceFolder);

            // ExcludePath can only be added or removed, not updated.

            // First remove any ExcludePaths that were removed from our model
            await RemoveExcludePathsNotInListAsync(transaction, fileSourceFolder.FileSourceFolderID, fileSourceFolder.ExcludePaths);

            // Now add any new ExcludePaths
            foreach (var excludePath in fileSourceFolder.ExcludePaths)
            {
                if (excludePath.ExcludePathID == 0)
                    await AddExcludePathAsync(transaction, fileSourceFolder.FileSourceFolderID, excludePath);
            }
        }




        // Removes any ExcludePaths not found in the provided set
        private async Task RemoveExcludePathsNotInListAsync(IDbTransaction transaction, int fileSourceFolderID, IEnumerable<ExcludePath> excludePathsToKeep)
        {
            HashSet<int> pathsToRemove = new HashSet<int>();

            const string allExcludePathIDsSql = @"SELECT [ExcludePathID] FROM [dbo].[ExcludePaths] WHERE FileSourceFolderID = @FileSourceFolderID";
            const string removeExcludePathSql = @"DELETE FROM ExcludePaths WHERE ExcludePathID = @ExcludePathID";

            // Get our current ExcludePath IDs in the database
            var results = await transaction.QueryAsync<int>(allExcludePathIDsSql, new { FileSourceFolderID = fileSourceFolderID });
            foreach (int id in results)
                pathsToRemove.Add(id);

            // Remove the IDs we want to keep so we are left with the folders to remove
            foreach (var excludePath in excludePathsToKeep)
                pathsToRemove.Remove(excludePath.ExcludePathID);

            // Remove the ExcludePaths
            foreach (int excludePathID in pathsToRemove)
            {
                await transaction.ExecuteAsync(removeExcludePathSql, new { ExcludePathID = excludePathID });
            }
        }
        #endregion
    }
}
