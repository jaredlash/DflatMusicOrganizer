using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dflat.Application.Services
{
    public class FileCollectionCompare : IFileCollectionCompare
    {
        public class CompareResult
        {
            //A collection of files that are new(the remaining files in the Staged files collection)
            public List<Models.File> Added { get; set; } = new List<Models.File>();

            //A collection of files that have been removed(the Removed files collection)
            public List<Models.File> Removed { get; set; } = new List<Models.File>();

            //A collection of files that have been replaced/modified(the Modified files collection)
            public List<Models.File> Modified { get; set; } = new List<Models.File>();

        }

        // TODO: This could stand to have some optimization on looking up and removing values from the "after" collection
        // First make it work, then make it faster.
        public CompareResult Compare(IEnumerable<Models.File> beforeFiles, IEnumerable<Models.File> after)
        {
            CompareResult result = new CompareResult();

            // Copy after collection it can be modified without modifying original
            var afterFiles = new List<Models.File>(after);

            var modifiedOrRemoved = new List<Models.File>();

            // Find unchanged files and remove them from consideration
            foreach (var beforeFile in beforeFiles)
            {
                int afterFileIndex = afterFiles.FindIndex(f => f.Filename == beforeFile.Filename
                    && f.Directory == beforeFile.Directory
                    && f.Size == beforeFile.Size
                    && f.LastModifiedTime == beforeFile.LastModifiedTime);

                // If found, remove from further consideration
                if (afterFileIndex >= 0)
                {
                    afterFiles.RemoveAt(afterFileIndex);
                }
                else // Otherwise, this file is changed or removed
                {
                    modifiedOrRemoved.Add(beforeFile);
                }
            }

            // Now separate out removed versus modified files
            foreach (var beforeFile in modifiedOrRemoved)
            {
                // Modified files are not matched on Size or LastModifiedTime, only Filename and Directory
                int afterFileIndex = afterFiles.FindIndex(f => f.Filename == beforeFile.Filename && f.Directory == beforeFile.Directory);

                // Modified file found
                if (afterFileIndex >= 0)
                {
                    var afterFile = afterFiles[afterFileIndex];
                    afterFile.FileID = beforeFile.FileID;
                    result.Modified.Add(afterFile);
                    afterFiles.RemoveAt(afterFileIndex);
                }
                else // Otherwise, it wasn't found as modified, so it must be counted as removed
                {
                    result.Removed.Add(beforeFile);
                }
            }

            // Remaining of afterFiles collection are the new files
            result.Added.AddRange(afterFiles);

            return result;
        }
    }
}
