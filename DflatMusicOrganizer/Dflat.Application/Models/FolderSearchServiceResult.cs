using System.Collections.Generic;

namespace Dflat.Application.Models
{
	public class FolderSearchServiceResult
	{
		private readonly List<FileResult> foundFiles;
		private readonly List<string> errorLog;

		public FolderSearchServiceResult()
		{
			foundFiles = new List<FileResult>();
			errorLog = new List<string>();
		}

		public IList<FileResult> FoundFiles
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

