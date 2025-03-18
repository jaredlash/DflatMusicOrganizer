using System.IO;

namespace Dflat.Application.Wrappers
{
    /// <summary>
    /// Wraps functionality of System.IO to provide an interface that can be mocked.
    /// </summary>
    public class SystemIOWrapper : ISystemIOWrapper
    {
        public IFileInfo GetFileInfo(string file)
        {
            return new FileInfoWrapper(file);
        }


        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }


        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }
    }
}
