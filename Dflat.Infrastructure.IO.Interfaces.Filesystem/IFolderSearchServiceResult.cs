using System;
using System.Collections.Generic;

namespace Dflat.Infrastructure.IO.Interfaces.Filesystem
{
	public interface IFolderSearchServiceResult
	{
		IList<string> FoundFiles {
			get;
		}

		IList<string> ErrorLog {
			get;
		}
	}
}

