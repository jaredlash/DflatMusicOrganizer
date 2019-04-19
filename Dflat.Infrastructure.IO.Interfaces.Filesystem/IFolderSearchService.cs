using System;
using System.Collections.Generic;

namespace Dflat.Infrastructure.IO.Interfaces.Filesystem
{
    public interface IFolderSearchService
	{
        IFolderSearchServiceResult FindFiles(string sourceDirectory, HashSet<string> excludeDirectories, Predicate<string> condition);

        IFolderSearchServiceResult FindFiles(string sourceDirectory, Predicate<string> condition);

		IFolderSearchServiceResult FindFiles(string sourceDirectory);
	}
}

