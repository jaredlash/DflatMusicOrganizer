using System;
using System.Collections.Generic;

namespace Infrastructure.IO.Interfaces.Filesystem
{
	public interface IFolderSearchServiceResult
	{
		IList<String> FoundFiles {
			get;
		}

		IList<String> ErrorLog {
			get;
		}
	}
}

