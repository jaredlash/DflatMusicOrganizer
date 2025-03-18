namespace Dflat.Application.Wrappers
{
    /// <summary>
    /// Wraps functionality of System.IO to provide an interface that can be mocked.
    /// </summary>
    public interface ISystemIOWrapper
    {
        bool DirectoryExists(string path);
        string[] GetDirectories(string path);
        IFileInfo GetFileInfo(string file);
        string[] GetFiles(string path);
    }
}