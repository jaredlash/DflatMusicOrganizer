using Dflat.Business.Models;
using Dflat.Infrastructure.IO.Filesystem;

namespace Dflat.Business
{
    public static class FileResultExtension
    {
        public static File CreateFile(this FileResult fileResult)
        {
            var file = new File
            {
                Filename = fileResult.Filename,
                Directory = fileResult.Directory,
                Extension = fileResult.Extension.ToLowerInvariant(),
                Size = fileResult.Size,
                LastModifiedTime = fileResult.LastModifiedTime
            };

            return file;
        }
    }
}
