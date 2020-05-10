using Dapper;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Dflat.Data.Dapper.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly string connectionString;

        public FileRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Add(File newFile)
        {
            const string sql = @"INSERT INTO [dbo].[Files]
                                            ([Filename]
                                            ,[Extension]
                                            ,[Directory]
                                            ,[Size]
                                            ,[LastModifiedTime]
                                            ,[MarkedAsRemoved]
                                            ,[MD5Sum])
                                      VALUES
                                            (@Filename
                                            ,@Extension
                                            ,@Directory
                                            ,@Size
                                            ,@LastModifiedTime
                                            ,0
                                            ,@MD5Sum);
                                 SELECT CAST(SCOPE_IDENTITY() as int);";

            using IDbConnection connection = new SqlConnection(connectionString);
            newFile.FileID = connection.QuerySingle<int>(sql, newFile);
        }

        public IEnumerable<File> GetFromPath(string path, bool recurse = true)
        {
            string sql = @"SELECT [FileID]
                                 ,[Filename]
                                 ,[Extension]
                                 ,[Directory]
                                 ,[Size]
                                 ,[LastModifiedTime]
                                 ,[MarkedAsRemoved]
                                 ,[MD5Sum]
                             FROM [dbo].[Files]
                            WHERE [MarkedAsRemoved] = 0";
            var queryParams = new DynamicParameters();
            if (recurse)
            {
                queryParams.Add("@Directory", EncodeForLike(path));
                sql += " AND Directory LIKE @Directory + '%'";
            }
            else
            {
                queryParams.Add("@Directory", path);
                sql += " AND Directory = @Directory";
            }

            using IDbConnection connection = new SqlConnection(connectionString);
            return connection.Query<File>(sql, queryParams);
        }

        public void MarkRemoved(int fileID)
        {
            const string sql = @"UPDATE [dbo].[Files]
                                    SET [MarkedAsRemoved] = 1
                                  WHERE [FileID] = @FileID";
            using IDbConnection connection = new SqlConnection(connectionString);
            int rowsAffected = connection.Execute(sql, new { FileID = fileID });

            if (rowsAffected == 0)
                throw new Exception($"File = {fileID} not found");
        }

        public void Update(File modifiedFile)
        {
            const string sql = @"UPDATE [dbo].[Files]
                                    SET [Filename] = @Filename
                                       ,[Extension] = @Extension
                                       ,[Directory] = @Directory
                                       ,[Size] = @Size
                                       ,[LastModifiedTime] = @LastModifiedTime
                                       ,[MD5Sum] = @MD5Sum
                                  WHERE [FileID] = @FileID";
            using IDbConnection connection = new SqlConnection(connectionString);
            int rowsAffected = connection.Execute(sql, modifiedFile);

            if (rowsAffected == 0)
                throw new Exception($"File = {modifiedFile.FileID} not found");
        }

        private string EncodeForLike(string input) => input.Replace("[", "[[]").Replace("%", "[%]");
    }
}
