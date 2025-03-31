using Dflat.Application.Models;
using Dflat.Application.Repositories;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace Dflat.Application.Services.JobServices;

public class MD5Service : JobService<MD5Job>
{
    private readonly IFileRepository fileRepository;

    public MD5Service(IFileRepository fileRepository, IJobRepository jobRepository, IBackgroundJobRunner<MD5Job> jobRunner) : base(jobRepository, jobRunner)
    {
        this.fileRepository = fileRepository;

        MaxConcurrentJobs = 20;
        AcceptedRequestTypes.Add(typeof(MD5Job));
    }

    public override void DoWork(MD5Job job, CancellationToken cancellationToken)
    {
        Models.File file;

        try
        {
            file = fileRepository.Get(job.FileID);
        }
        catch (Exception ex)
        {
            job.Errors = $"Could not load file = {job.FileID}. {ex.Message}";
            job.Status = JobStatus.Error;
            return;
        }

        string fullFilePath = string.Join(Path.DirectorySeparatorChar, file.Directory, file.Filename);
        string result;

        try
        {
            using FileStream stream = System.IO.File.OpenRead(fullFilePath);
            MD5 md5 = MD5.Create();
            byte[] checksum = md5.ComputeHash(stream);
            result = BitConverter.ToString(checksum).Replace("-", string.Empty).ToLower();
        }
        catch (Exception ex)
        {
            job.Errors = $"Could not compute MD5: {ex.Message}";
            job.Status = JobStatus.Error;
            return;
        }

        job.Output = "MD5: " + result;
        job.Status = JobStatus.Success;

        fileRepository.UpdateMD5(file.FileID, result);
    }

    public override void QueuePrerequisites(MD5Job job)
    {
        // No pre-requisites
    }
}
