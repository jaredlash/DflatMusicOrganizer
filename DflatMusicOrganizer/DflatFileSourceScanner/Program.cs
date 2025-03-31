using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Dflat.Application.Services;
using Dflat.Application.Wrappers;

namespace DflatFileSourceScanner;

class Program
{
    static bool MusicFilter(string filename)
    {
        string extension;
        HashSet<string> validExtensions = new() { ".aiff", ".flac", ".m4a", ".mp2", ".mp3", ".ogg", ".wav", ".wma" };
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
        IFolderSearchService folderSearch = new FolderSearchService(new SystemIOWrapper(), MusicFilter);

        var excludeDirectories = args.Skip(1);

        var result = folderSearch.FindFiles(args[0], [.. excludeDirectories], MusicFilter, new System.Threading.CancellationToken());

        if (result.ErrorLog.Count > 0)
        {
            foreach (var error in result.ErrorLog)
                Console.WriteLine("Error: {0}", error);

            return 1;
        }

        foreach (var foundFile in result.FoundFiles)
            Console.WriteLine("{0}: {1}", foundFile.Extension.ToUpperInvariant(), Path.Combine(foundFile.Directory, foundFile.Filename));

        Console.WriteLine();
        Console.WriteLine($"Found {result.FoundFiles.Count} files.");
        Console.WriteLine($"{result.ErrorLog.Count} errors were encountered.");
        return 0;
    }
}
