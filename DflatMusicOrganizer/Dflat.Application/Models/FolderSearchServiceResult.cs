using System.Collections.Generic;

namespace Dflat.Application.Models;

public class FolderSearchServiceResult
{
    private readonly List<FileResult> foundFiles;
    private readonly List<string> errorLog;

    public FolderSearchServiceResult()
    {
        foundFiles = [];
        errorLog = [];
    }

    public IList<FileResult> FoundFiles => foundFiles;

    public IList<string> ErrorLog => errorLog;
}