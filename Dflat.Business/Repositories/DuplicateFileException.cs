using System;
using Dflat.Business.Models;

namespace Dflat.Business.Repositories
{
    public class DuplicateFileException : Exception
    {
        private File file;

        public DuplicateFileException(File file, string message) : base(message)
        {
            this.file = file;
        }
    }
}