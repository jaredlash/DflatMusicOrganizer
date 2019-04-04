using System;
using System.Collections.Generic;
using Dflat.Infrastructure.IO.Filesystem;
using Dflat.Infrastructure.IO.Interfaces.Filesystem;
using System.IO;

namespace DflatFileSourceScanner
{
    class Program
    {
        static bool MusicFilter(string filename)
        {
            string extension;
            HashSet<string> validExtensions = new HashSet<string>(){ ".aiff", ".flac", ".m4a", ".mp2", ".mp3", ".ogg", ".wav", ".wma"};
            try
            {
                extension = Path.GetExtension(filename).ToLowerInvariant();
            }
            catch (ArgumentException)
            {
                return false;
            }

            if (validExtensions.Contains(extension))
                return true;

            return false;
        }


        static int Main(string[] args)
        {
            IFolderSearchService folderSearch = new FolderSearchService(MusicFilter);

            foreach (var arg in args)
            {
                IFolderSearchServiceResult result = folderSearch.FindFiles(args[0]);

                if (result.ErrorLog.Count > 0)
                {
                    foreach (var error in result.ErrorLog)
                        Console.WriteLine("Error: {0}", error);

                    return 1;
                }

                foreach (var foundFile in result.FoundFiles)
                    Console.WriteLine(foundFile);
            }

            return 0;
        }
    }
}
