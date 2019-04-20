using System;
using System.Collections.Generic;

namespace Dflat.Infrastructure.IO.Filesystem
{
    public interface IFolderSearchService
	{
        FolderSearchServiceResult FindFiles(string sourceDirectory, HashSet<string> excludeDirectories, Predicate<string> condition);

        FolderSearchServiceResult FindFiles(string sourceDirectory, Predicate<string> condition);

		FolderSearchServiceResult FindFiles(string sourceDirectory);
	}
}

