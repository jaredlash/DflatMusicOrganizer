using Dflat.Application.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Dflat.Application.Services
{
    public interface IFolderSearchService
    {
        FolderSearchServiceResult FindFiles(string sourceDirectory, HashSet<string> excludeDirectories, Predicate<string> condition, CancellationToken cancellationToken);

        FolderSearchServiceResult FindFiles(string sourceDirectory, Predicate<string> condition, CancellationToken cancellationToken);

        FolderSearchServiceResult FindFiles(string sourceDirectory, CancellationToken cancellationToken);
    }
}

