using Dflat.Application.Models;
using System.Collections.Generic;

namespace Dflat.Application.Services;

public interface IFileCollectionCompare
{
    FileCollectionCompare.CompareResult Compare(IEnumerable<File> beforeFiles, IEnumerable<File> after);
}