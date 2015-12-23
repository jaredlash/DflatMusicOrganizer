using System;
using System.Collections.Generic;
using System.IO;

namespace Infrastructure.IO.Interfaces.Filesystem
{
	public interface IFolderSearchService
	{
		IFolderSearchServiceResult FindFiles (String sourceDirectory, Predicate<String> condition);

		IFolderSearchServiceResult FindFiles (String sourceDirectory);
	}
}

