﻿using Dapper;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Dflat.Data.Dapper.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly string connectionString;

        public JobRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Add<JobType>(JobType job) where JobType : Job
        {
            string sql = @"INSERT INTO [dbo].[Jobs]
                                      ([CreationTime]
                                      ,[Description]
                                      ,[IgnoreCache]
                                      ,[Status]
                                      ,[Output]
                                      ,[Errors]
                                      ,[JobType]
                                      ,[FileSourceFolderID])
                                VALUES
                                      (@CreationTime
                                      ,@Description
                                      ,@IgnoreCache
                                      ,@Status
                                      ,@Output
                                      ,@Errors
                                      ,@JobType
                                      ,@FileSourceFolderID);
                           SELECT CAST(SCOPE_IDENTITY() as int);";

            var model = new DynamicParameters(job);
            model.RemoveUnused = true; // Remove JobID

            switch (job)
            {
                case FileSourceFolderScanJob j:
                    model.Add("@JobType", Application.Models.JobType.FileSourceFolderScanJob);
                    break;

                default:
                    throw new NotImplementedException();
            }

            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                job.JobID = conn.QuerySingle<int>(sql, model);
            }
        }

        public bool CancelJob(int jobID)
        {
            string sql = @"UPDATE [dbo].[Jobs]
                              SET [Status] = @Status
                            WHERE JobID = @JobID
                              AND [Status] NOT IN @InvalidCancelStatuses";

            var model = new
            {
                JobID = jobID,
                Status = JobStatus.Cancelled,
                InvalidCancelStatuses = new[] { JobStatus.Success, JobStatus.SuccessWithErrors }
            };

            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                int rowsAffected = conn.Execute(sql, model);

                if (rowsAffected == 0)
                    return false;
            }

            return true;
        }

        public Job Get(int jobID)
        {
            Job result;

            string sql = @"SELECT * FROM Jobs WHERE JobID = @JobID";

            using (IDbConnection conn = new SqlConnection(connectionString))
            using (var reader = conn.ExecuteReader(sql, new { JobID = jobID }))
            {
                var fileSourceFolderScanJobParser = reader.GetRowParser<FileSourceFolderScanJob>();
                if (reader.Read())
                {
                    var discriminator = (JobType)reader.GetInt32(reader.GetOrdinal(nameof(JobType)));
                    switch (discriminator)
                    {
                        case JobType.FileSourceFolderScanJob:
                            result = fileSourceFolderScanJobParser(reader);
                            break;

                        default:
                            result = null;
                            break;
                    }
                }
                else
                    return null;
            }

            return result;
        }

        public JobInfo GetJobInfo(int jobID)
        {
            string sql = @"SELECT [JobID]
                                 ,[CreationTime]
                                 ,[Description]
                                 ,[IgnoreCache]
                                 ,[Status]
                                 ,[JobType]
                             FROM [DflatMusicOrganizer].[dbo].[Jobs]
                            WHERE [JobID] = @JobID";

            using IDbConnection conn = new SqlConnection(connectionString);
            return conn.QuerySingleOrDefault<JobInfo>(sql, new { JobID = jobID });
        }

        public IEnumerable<JobInfo> GetJobInfoByCriteria(JobType jobType = JobType.None, JobStatus status = JobStatus.None)
        {
            string sql = @"SELECT [JobID]
                                 ,[CreationTime]
                                 ,[Description]
                                 ,[IgnoreCache]
                                 ,[Status]
                                 ,[JobType]
                             FROM [DflatMusicOrganizer].[dbo].[Jobs]";

            var whereCriteria = new DynamicParameters();
            List<string> whereClauses = new List<string>();

            if (jobType != JobType.None)
            {
                whereClauses.Add("JobType = @JobType");
                whereCriteria.Add("@JobType", (int)jobType);
            }

            if (status != JobStatus.None)
            {
                whereClauses.Add("Status = @Status");
                whereCriteria.Add("@Status", (int)status);
            }

            if (whereClauses.Count > 0)
            {
                sql += " WHERE " + string.Join(" AND ", whereClauses);
            }
            sql += " ORDER BY CreationTime DESC";

            using IDbConnection conn = new SqlConnection(connectionString);
            return conn.Query<JobInfo>(sql, (object)whereCriteria);
        }

        public JobType GetNextAvailable<JobType>() where JobType : Job
        {
            Job nextJob;

            string findSql = @"SELECT * FROM Jobs WHERE JobType = @JobType AND Status IN @JobStatuses";

            string updateSql = @"UPDATE [dbo].[Jobs]
                                    SET [Status] = @Status
                                  WHERE JobID = @JobID";

            // Set up our query parameters
            var queryParams = new DynamicParameters();

            if (typeof(JobType) == typeof(FileSourceFolderScanJob))
                queryParams.Add("@JobType", Application.Models.JobType.FileSourceFolderScanJob);
            else
                queryParams.Add("@JobType", Application.Models.JobType.None);

            queryParams.Add("@JobStatuses", new[] { JobStatus.Ready, JobStatus.Running });


            
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                // Get the running and ready jobs
                List<Job> runningJobs = new List<Job>();
                List<Job> readyJobs = new List<Job>();

                using (var reader = conn.ExecuteReader(findSql, queryParams))
                {
                    var fileSourceFolderScanJobParser = reader.GetRowParser<FileSourceFolderScanJob>();
                    while (reader.Read())
                    {
                        Job foundJob;

                        var discriminator = (Application.Models.JobType)reader.GetInt32(reader.GetOrdinal(nameof(Application.Models.JobType)));
                        switch (discriminator)
                        {
                            case Application.Models.JobType.FileSourceFolderScanJob:
                                foundJob = fileSourceFolderScanJobParser(reader);
                                break;

                            default:
                                throw new NotImplementedException();
                        }

                        if (foundJob.Status == JobStatus.Running)
                            runningJobs.Add(foundJob);
                        else
                            readyJobs.Add(foundJob);
                    }
                }

                // Find our next job based on what is currently running and what's available
                nextJob = readyJobs.Where((j) => runningJobs.Any((r) => r.SameRequestAs(j)) == false).FirstOrDefault();
                if (nextJob == null)
                    return null;

                // Update status to running and return the found job
                int rowsAffected = conn.Execute(updateSql, new { nextJob.JobID, Status = JobStatus.Running });

                if (rowsAffected == 0)
                    return null;
            }

            return nextJob as JobType;
        }

        public bool RestartJob(int jobID)
        {
            string sql = @"UPDATE [dbo].[Jobs]
                              SET [Status] = @Status
                            WHERE JobID = @JobID
                              AND [Status] NOT IN @InvalidRestartStatuses";

            var model = new DynamicParameters();
            model.Add("@JobID", jobID);
            model.Add("@Status", JobStatus.Ready);
            model.Add("@InvalidRestartStatuses", new[] { JobStatus.Running, JobStatus.Queued, JobStatus.Ready });

            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                int rowsAffected = conn.Execute(sql, model);

                if (rowsAffected == 0)
                    return false;
            }

            return true;
        }

        public void Update<JobType>(JobType job) where JobType : Job
        {
            string sql = @"UPDATE [dbo].[Jobs]
                              SET [Description] = @Description
                                 ,[IgnoreCache] = @IgnoreCache
                                 ,[Status] = @Status
                                 ,[Output] = @Output
                                 ,[Errors] = @Errors
                                 ,[JobType] = @JobType
                                 ,[FileSourceFolderID] = @FileSourceFolderID
                            WHERE JobID = @JobID";

            var model = new DynamicParameters(job);

            switch (job)
            {
                case FileSourceFolderScanJob folderScanJob:
                    model.Add("JobType", Application.Models.JobType.FileSourceFolderScanJob);
                    break;

                default:
                    throw new NotImplementedException();
            }

            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                int rowsAffected = conn.Execute(sql, (object)model);

                if (rowsAffected == 0)
                    throw new Exception($"Job = {job.JobID} not found");
            }
        }
    }
}