using Dflat.Application.Models;
using System;
using System.Collections.Generic;

namespace Dflat.Application.Services
{
    public interface IFolderSearchService
    {
        FolderSearchServiceResult FindFiles(string sourceDirectory, HashSet<string> excludeDirectories, Predicate<string> condition);

        FolderSearchServiceResult FindFiles(string sourceDirectory, Predicate<string> condition);

        FolderSearchServiceResult FindFiles(string sourceDirectory);
    }
}

