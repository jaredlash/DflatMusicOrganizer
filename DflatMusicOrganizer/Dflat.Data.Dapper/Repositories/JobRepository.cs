using Dapper;
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
    public class JobRepository : IJobRepository
    {
        private readonly string connectionString;

        public JobRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void Add<JobType>(JobType job) where JobType : Job
        {
            const string sql = @"INSERT INTO [dbo].[Jobs]
                                            ([CreationTime]
                                            ,[Description]
                                            ,[IgnoreCache]
                                            ,[Status]
                                            ,[Output]
                                            ,[Errors]
                                            ,[JobType]
                                            ,[FileSourceFolderID]
                                            ,[FileID])
                                      VALUES
                                            (@CreationTime
                                            ,@Description
                                            ,@IgnoreCache
                                            ,@Status
                                            ,@Output
                                            ,@Errors
                                            ,@JobType
                                            ,@FileSourceFolderID
                                            ,@FileID);
                                 SELECT CAST(SCOPE_IDENTITY() as int);";

            var model = new DynamicParameters(job)
            {
                RemoveUnused = true // Remove JobID
            };

            switch (job)
            {
                case FileSourceFolderScanJob _:
                    model.Add("@JobType", Application.Models.JobType.FileSourceFolderScanJob);
                    model.Add("@FileID", null);
                    break;

                case MD5Job _:
                    model.Add("@JobType", Application.Models.JobType.MD5Job);
                    model.Add("@FileSourceFolderID", null);
                    break;

                default:
                    throw new NotImplementedException();
            }

            using IDbConnection conn = new SqlConnection(connectionString);
            job.JobID = conn.QuerySingle<int>(sql, model);
        }

        public bool CancelJob(int jobID)
        {
            const string sql = @"UPDATE [dbo].[Jobs]
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

            const string sql = @"SELECT * FROM Jobs WHERE JobID = @JobID";

            using (IDbConnection conn = new SqlConnection(connectionString))
            using (var reader = conn.ExecuteReader(sql, new { JobID = jobID }))
            {
                var fileSourceFolderScanJobParser = reader.GetRowParser<FileSourceFolderScanJob>();
                var md5JobParser = reader.GetRowParser<MD5Job>();
                if (reader.Read())
                {
                    var discriminator = (JobType)reader.GetInt32(reader.GetOrdinal(nameof(JobType)));
                    result = discriminator switch
                    {
                        JobType.FileSourceFolderScanJob => fileSourceFolderScanJobParser(reader),
                        JobType.MD5Job => md5JobParser(reader),
                        _ => null,
                    };
                }
                else
                    return null;
            }

            return result;
        }

        public JobInfo GetJobInfo(int jobID)
        {
            const string sql = @"SELECT [JobID]
                                       ,[CreationTime]
                                       ,[Description]
                                       ,[IgnoreCache]
                                       ,[Status]
                                       ,[JobType]
                                   FROM [dbo].[Jobs]
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
                             FROM [dbo].[Jobs]";

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


        public async Task<IEnumerable<JobInfo>> GetJobInfoByCriteriaAsync(JobType jobType = JobType.None, JobStatus status = JobStatus.None)
        {
            string sql = @"SELECT [JobID]
                                 ,[CreationTime]
                                 ,[Description]
                                 ,[IgnoreCache]
                                 ,[Status]
                                 ,[JobType]
                             FROM [dbo].[Jobs]";

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
            return await conn.QueryAsync<JobInfo>(sql, (object)whereCriteria);
        }


        public JobType GetNextAvailable<JobType>() where JobType : Job
        {
            Job nextJob;

            const string findSql = @"SELECT * FROM Jobs WHERE JobType = @JobType AND Status IN @JobStatuses";

            const string updateSql = @"UPDATE [dbo].[Jobs]
                                          SET [Status] = @Status
                                        WHERE JobID = @JobID";

            // Set up our query parameters
            var queryParams = new DynamicParameters();

            if (typeof(JobType) == typeof(FileSourceFolderScanJob))
                queryParams.Add("@JobType", Application.Models.JobType.FileSourceFolderScanJob);
            else if (typeof(JobType) == typeof(MD5Job))
                queryParams.Add("@JobType", Application.Models.JobType.MD5Job);
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
                    var md5JobParser = reader.GetRowParser<MD5Job>();
                    while (reader.Read())
                    {
                        var discriminator = (Application.Models.JobType)reader.GetInt32(reader.GetOrdinal(nameof(Application.Models.JobType)));
                        Job foundJob = discriminator switch
                        {
                            Application.Models.JobType.FileSourceFolderScanJob => fileSourceFolderScanJobParser(reader),
                            Application.Models.JobType.MD5Job => md5JobParser(reader),
                            _ => throw new NotImplementedException(),
                        };
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

        public int GetJobCount()
        {
            const string sql = @"SELECT COUNT(*) FROM Jobs";
            using IDbConnection connection = new SqlConnection(connectionString);

            return connection.QuerySingle<int>(sql);
        }

        public int GetJobCountByStatus(JobStatus jobStatus)
        {
            const string sql = @"SELECT COUNT(*) FROM Jobs WHERE Status = @JobStatus";
            using IDbConnection connection = new SqlConnection(connectionString);

            return connection.QuerySingle<int>(sql, new { JobStatus = jobStatus });
        }

        public int GetQueuedJobCount()
        {
            const string sql = @"SELECT COUNT(*) FROM Jobs WHERE Status IN @JobStatuses";
            using IDbConnection connection = new SqlConnection(connectionString);

            return connection.QuerySingle<int>(sql, new { JobStatuses = new[] { JobStatus.Ready, JobStatus.Queued } });
        }

        public int GetRunningJobCount()
        {
            const string sql = @"SELECT COUNT(*) FROM Jobs WHERE Status = @JobStatus";
            using IDbConnection connection = new SqlConnection(connectionString);

            return connection.QuerySingle<int>(sql, new { JobStatus = JobStatus.Running });
        }

        public bool RestartJob(int jobID)
        {
            const string sql = @"UPDATE [dbo].[Jobs]
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
            const string sql = @"UPDATE [dbo].[Jobs]
                                    SET [Description] = @Description
                                       ,[IgnoreCache] = @IgnoreCache
                                       ,[Status] = @Status
                                       ,[Output] = @Output
                                       ,[Errors] = @Errors
                                       ,[JobType] = @JobType
                                       ,[FileSourceFolderID] = @FileSourceFolderID
                                       ,[FileID] = @FileID
                                  WHERE JobID = @JobID";

            var model = new DynamicParameters(job);

            switch (job)
            {
                case FileSourceFolderScanJob _:
                    model.Add("@JobType", Application.Models.JobType.FileSourceFolderScanJob);
                    model.Add("@FileID", null);
                    break;

                case MD5Job _:
                    model.Add("@JobType", Application.Models.JobType.MD5Job);
                    model.Add("@FileSourceFolderID", null);
                    break;

                default:
                    throw new NotImplementedException();
            }

            using IDbConnection conn = new SqlConnection(connectionString);
            int rowsAffected = conn.Execute(sql, (object)model);

            if (rowsAffected == 0)
                throw new Exception($"Job = {job.JobID} not found");
        }
    }
}
