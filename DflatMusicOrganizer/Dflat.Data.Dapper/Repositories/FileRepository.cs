using Dapper;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Dflat.Data.Dapper.Repositories;

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
                                            ([FileID]
                                            ,[Filename]
                                            ,[Extension]
                                            ,[Directory]
                                            ,[Size]
                                            ,[LastModifiedTime]
                                            ,[MarkedAsRemoved]
                                            ,[MD5Sum])
                                      VALUES
                                            (@FileID
                                            ,@Filename
                                            ,@Extension
                                            ,@Directory
                                            ,@Size
                                            ,@LastModifiedTime
                                            ,0
                                            ,@MD5Sum);";

        using IDbConnection connection = new SqlConnection(connectionString);
        connection.Execute(sql, newFile);
    }

    public File Get(Guid fileID)
    {
        const string sql = @"SELECT [FileID]
                                       ,[Filename]
                                       ,[Extension]
                                       ,[Directory]
                                       ,[Size]
                                       ,[LastModifiedTime]
                                       ,[MarkedAsRemoved]
                                       ,[MD5Sum]
                                   FROM [dbo].[Files]
                                  WHERE [FileID] = @FileID";
        
        using IDbConnection connection = new SqlConnection(connectionString);
        return connection.QuerySingle<File>(sql, new { FileID = fileID });
    }

    public IEnumerable<File> GetFromPath(string path, IEnumerable<string> excludePaths, bool recurse = true)
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

            int i = 0;
            foreach(var excludePath in excludePaths)
            {
                queryParams.Add($"@ExcludeDir{i}", EncodeForLike(excludePath));
                sql += $" AND Directory NOT LIKE @ExcludeDir{i} + '%'";
                i++;
            }
        }
        else
        {
            // Only search on specified directory
            // Exclude paths are not valid unless search includes sub-directories
            queryParams.Add("@Directory", path);
            sql += " AND Directory = @Directory";
        }

        using IDbConnection connection = new SqlConnection(connectionString);
        return connection.Query<File>(sql, queryParams);
    }

    public void MarkRemoved(Guid fileID)
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


    public void UpdateMD5(Guid fileID, string md5)
    {
        const string sql = @"UPDATE [dbo].[Files]
                                    SET [MD5Sum] = @MD5Sum
                                  WHERE [FileID] = @FileID";

        using IDbConnection connection = new SqlConnection(connectionString);
        int rowsAffected = connection.Execute(sql, new { MD5Sum = md5, FileID = fileID });

        if (rowsAffected == 0)
            throw new Exception($"File = {fileID} not found");
    }

    private string EncodeForLike(string input) => input.Replace("[", "[[]").Replace("%", "[%]");
}
