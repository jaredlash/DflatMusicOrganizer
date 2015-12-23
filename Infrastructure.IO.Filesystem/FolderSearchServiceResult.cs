using System;
using Infrastructure.IO.Interfaces.Filesystem;
using System.Collections.Generic;

namespace Infrastructure.IO.Filesystem
{
	public class FolderSearchServiceResult : IFolderSearchServiceResult
	{
		private readonly List<String> foundFiles;
		private readonly List<String> errorLog;

		public FolderSearchServiceResult ()
		{
			foundFiles = new List<String> ();
			errorLog = new List<String> ();
		}

		public IList<String> FoundFiles
		{
			get
			{
				return foundFiles;
			}
		}

		public IList<String> ErrorLog
		{
			get
			{
				return errorLog;
			}
		}
	}
}

