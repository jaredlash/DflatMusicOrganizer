using System;
using System.Collections.Generic;
using System.IO;

namespace Dflat.Infrastructure.IO.Interfaces.Filesystem
{
	public interface IFolderSearchService
	{
		IFolderSearchServiceResult FindFiles (string sourceDirectory, Predicate<string> condition);

		IFolderSearchServiceResult FindFiles (string sourceDirectory);
	}
}

