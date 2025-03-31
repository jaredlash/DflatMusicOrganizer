using Dflat.Application.Models;
using Dflat.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Dflat.Application.Services;

public class FolderSearchService : IFolderSearchService
{
    private readonly ISystemIOWrapper systemIOWrapper;
    private readonly Predicate<string> _searchFilter;

    public FolderSearchService(ISystemIOWrapper systemIOWrapper) : this(systemIOWrapper, (f) => true)
    {
    }

    public FolderSearchService(ISystemIOWrapper systemIOWrapper, Predicate<string> condition)
    {
        this.systemIOWrapper = systemIOWrapper;
        _searchFilter = condition;
    }

    public FolderSearchServiceResult FindFiles(string sourceDirectory,
                                               HashSet<string> excludeDirectories,
                                               Predicate<string> condition,
                                               CancellationToken cancellationToken)
    {
        Stack<string> dirs = new Stack<string>(20);
        FolderSearchServiceResult result = new FolderSearchServiceResult();

        try
        {

            if (!systemIOWrapper.DirectoryExists(sourceDirectory))
                throw new System.IO.DirectoryNotFoundException($"Directory does not exist: {sourceDirectory}");

            if (!excludeDirectories.Contains(sourceDirectory)) dirs.Push(sourceDirectory);

            while (dirs.Count > 0)
            {
                string currentDirectory = dirs.Pop();
                string[] subDirectories;

                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    subDirectories = systemIOWrapper.GetDirectories(currentDirectory);
                }
                // An UnauthorizedAccessException will be thrown if we do not have
                // discovery permission on a folder or file.  It may or may not be acceptable
                // to ignore the exception and continue enumerating the remaining files and
                // folders.  It is also possible (but unlikely) that a DirectoryNotFound exception
                // will be raised.  This will happen if currentDirectory has been deleted by
                // another application or thread after our call to Directory.Exists.  The
                // choice of which exceptions to catch depends entirely on the specific task
                // you are intending to perform and also on how much you know with certainty
                // about the systems on which this code will run.
                catch (Exception e) when (e is UnauthorizedAccessException || e is System.IO.DirectoryNotFoundException)
                {
                    result.ErrorLog.Add($"{currentDirectory} -- {e.Message}");
                    continue;
                }

                cancellationToken.ThrowIfCancellationRequested();

                // Push the subdirectories onto the stack for later traversal.
                foreach (string str in subDirectories)
                    if (!excludeDirectories.Contains(str)) dirs.Push(str);


                // Get the files and process them
                string[] files;
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    files = systemIOWrapper.GetFiles(currentDirectory);
                }
                catch (Exception e) when (e is UnauthorizedAccessException || e is System.IO.DirectoryNotFoundException)
                {

                    result.ErrorLog.Add($"{currentDirectory} -- {e.Message}");
                    continue;
                }


                foreach (string file in files)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        // Process found file
                        if (condition.Invoke(file))
                        {
                            IFileInfo fileInfo = systemIOWrapper.GetFileInfo(file);
                            result.FoundFiles.Add(new FileResult(fileInfo.Name,
                                            fileInfo.DirectoryName,
                                            fileInfo.Extension,
                                            fileInfo.Length,
                                            TruncateSeconds(fileInfo.LastWriteTime)));
                        }
                    }
                    catch (Exception e)
                    when (e is UnauthorizedAccessException ||
                          e is System.IO.DirectoryNotFoundException ||
                          e is System.IO.FileNotFoundException)
                    {
                        // If file was deleted by a separate application then just continue.
                        result.ErrorLog.Add($"{file} -- {e.Message}");
                        continue;
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            result.ErrorLog.Add("Cancelled");
        }

        return result;
    }

    public FolderSearchServiceResult FindFiles(string sourceDirectory, Predicate<string> condition, CancellationToken cancellationToken)
    {
        return FindFiles(sourceDirectory, new HashSet<string>(), condition, cancellationToken);
    }

    public FolderSearchServiceResult FindFiles(string sourceDirectory, CancellationToken cancellationToken)
    {
        return FindFiles(sourceDirectory, new HashSet<string>(), _searchFilter, cancellationToken);
    }

    private DateTime TruncateSeconds(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
    }
}

