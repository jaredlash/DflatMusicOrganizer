namespace Dflat.Application.Models
{
    public enum JobStatus
    {
        Queued = 1,
        Ready = 2,
        Running = 3,
        Success = 4,
        SuccessWithErrors = 5,
        Error = 6
    }
}