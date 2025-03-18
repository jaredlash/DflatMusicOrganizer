using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dflat.Application.Wrappers
{
    public class FileInfoWrapper : IFileInfo
    {
        private readonly FileInfo fileInfo;

        public FileInfoWrapper(string file)
        {
            fileInfo = new FileInfo(file);
        }

        public string Name => fileInfo.Name;

        public string DirectoryName => fileInfo.DirectoryName;

        public string Extension => fileInfo.Extension;

        public long Length => fileInfo.Length;

        public DateTime LastWriteTime => fileInfo.LastWriteTime;
    }
}
