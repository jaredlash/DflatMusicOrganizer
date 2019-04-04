using System;
using Dflat.Infrastructure.IO.Interfaces.Filesystem;
using System.Collections.Generic;

namespace Dflat.Infrastructure.IO.Filesystem
{
	public class FolderSearchServiceResult : IFolderSearchServiceResult
	{
		private readonly List<string> foundFiles;
		private readonly List<string> errorLog;

		public FolderSearchServiceResult ()
		{
			foundFiles = new List<string> ();
			errorLog = new List<string> ();
		}

		public IList<string> FoundFiles
		{
			get
			{
				return foundFiles;
			}
		}

		public IList<string> ErrorLog
		{
			get
			{
				return errorLog;
			}
		}
	}
}

