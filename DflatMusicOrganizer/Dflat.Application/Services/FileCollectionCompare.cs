using System.Collections.Generic;
using Dflat.Application.Models;
using System.Linq;

namespace Dflat.Application.Services;

public class FileCollectionCompare : IFileCollectionCompare
{
    public class CompareResult
    {
        // A collection of files that are new
        public List<File> Added { get; set; } = [];

        // A collection of files that have been removed
        public List<File> Removed { get; set; } = [];

        // A collection of files that have been replaced/modified
        public List<File> Modified { get; set; } = [];

    }

    public CompareResult Compare(IEnumerable<File> beforeFiles, IEnumerable<File> afterFiles)
    {
        CompareResult result = new CompareResult();


        if (!beforeFiles.Any() && !afterFiles.Any())
        {
            return result; // Nothing to compare
        }
        if (!beforeFiles.Any())
        {
            result.Added.AddRange(afterFiles);
            return result; // Only new files
        }
        if (!afterFiles.Any())
        {
            result.Removed.AddRange(beforeFiles);
            return result; // Only removed files
        }

        // Sort by default comparison, which is by Directory, Filename, LastModifiedTime, and Size
        var sortedBeforeFiles = beforeFiles.OrderBy(f => f);
        var sortedAfterFiles = afterFiles.OrderBy(f => f);

        var beforeEnumerator = sortedBeforeFiles.GetEnumerator();
        var afterEnumerator = sortedAfterFiles.GetEnumerator();

        bool beforeHasNext = beforeEnumerator.MoveNext();
        bool afterHasNext = afterEnumerator.MoveNext();

        while (beforeHasNext && afterHasNext)
        {
            var beforeFile = beforeEnumerator.Current;
            var afterFile = afterEnumerator.Current;

            int comparison = File.PathOnlyCompare(beforeFile, afterFile);

            if (comparison < 0)
            {
                // beforeFile is not in afterFiles, so it was removed
                result.Removed.Add(beforeFile);
                beforeHasNext = beforeEnumerator.MoveNext();
            }
            else if (comparison > 0)
            {
                // afterFile is not in beforeFiles, so it was added
                result.Added.Add(afterFile);
                afterHasNext = afterEnumerator.MoveNext();
            }
            else
            {
                // Files are equal in terms of sorting, check for modifications
                if (!beforeFile.Equals(afterFile))
                {
                    result.Modified.Add(afterFile);
                }
                beforeHasNext = beforeEnumerator.MoveNext();
                afterHasNext = afterEnumerator.MoveNext();
            }
        }

        // Add remaining files in beforeFiles to Removed
        while (beforeHasNext)
        {
            result.Removed.Add(beforeEnumerator.Current);
            beforeHasNext = beforeEnumerator.MoveNext();
        }

        // Add remaining files in afterFiles to Added
        while (afterHasNext)
        {
            result.Added.Add(afterEnumerator.Current);
            afterHasNext = afterEnumerator.MoveNext();
        }

        return result;
    }
}
