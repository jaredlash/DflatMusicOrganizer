using System;
using System.Collections.Generic;
using System.IO;
using Dflat.Infrastructure.IO.Interfaces.Filesystem;

namespace Dflat.Infrastructure.IO.Filesystem
{
	public class FolderSearchService : IFolderSearchService
	{
        private readonly Predicate<string> _searchFilter;

		public FolderSearchService ()
		{
            _searchFilter = (string f) => true;
        }

        public FolderSearchService (Predicate<string> condition)
        {
            _searchFilter = condition;
        }

		public IFolderSearchServiceResult FindFiles (string sourceDirectory, HashSet<string> excludeDirectories, Predicate<string> condition)
		{
			Stack<string> dirs = new Stack<string> (20);
			FolderSearchServiceResult result = new FolderSearchServiceResult ();

			if (!Directory.Exists (sourceDirectory))
                new DirectoryNotFoundException(string.Format("Directory does not exist: {1}", sourceDirectory));
            else
            { 
                if (!excludeDirectories.Contains(sourceDirectory)) dirs.Push(sourceDirectory);
            }

            while (dirs.Count > 0)
			{
                string currentDirectory = dirs.Pop ();
                string[] subDirectories;

				try
				{
					subDirectories = Directory.GetDirectories(currentDirectory);
				}
				// An UnauthorizedAccessException will be thrown if we do not have
				// discovery permission on a folder or file.  It may or may not be acceptable
				// to ignore the exception and continue enumerating the remaining files and
				// folders.  It is also possible (but unlikely) that a DirectoryNotFound exception
				// will be raised.  This will happen if currentDirectory has been deleted by
				// another application or thread after our call to Directory.Exists.  The
				// choice of which exceptions to catch depends entirely on the specific task
				// you are intending to perform and also on how much you know with certainty
				// about the systems on which this code will run.
				catch (UnauthorizedAccessException e)
				{
					result.ErrorLog.Add (e.Message);
					continue;
				}
				catch (System.IO.DirectoryNotFoundException e)
				{
					result.ErrorLog.Add (e.Message);
					continue;
				}

				string[] files = null;
				try
				{
					files = Directory.GetFiles(currentDirectory);
				}
				catch (UnauthorizedAccessException e)
				{

					result.ErrorLog.Add (e.Message);
					continue;
				}

				catch (System.IO.DirectoryNotFoundException e)
				{
					result.ErrorLog.Add (e.Message);
					continue;
				}
				// Perform the required action on each file here. 
				// Modify this block to perform your required task. 
				foreach (string file in files)
				{
					try
					{
						// Perform whatever action is required in your scenario.
						if (condition.Invoke(file))
							result.FoundFiles.Add(file);
					}
					catch (System.IO.FileNotFoundException e)
					{
						// If file was deleted by a separate application 
						// or thread since the call to TraverseTree() 
						// then just continue.
						result.ErrorLog.Add (e.Message);
						continue;
					}
				}

				// Push the subdirectories onto the stack for traversal. 
				// This could also be done before handing the files. 
				foreach (string str in subDirectories)
					if (!excludeDirectories.Contains(str)) dirs.Push(str);

			}

			return result;
		}

        public IFolderSearchServiceResult FindFiles(string sourceDirectory, Predicate<string> condition)
        {
            return FindFiles(sourceDirectory, new HashSet<string>(), condition);
        }

        public IFolderSearchServiceResult FindFiles (string sourceDirectory)
		{
			return FindFiles(sourceDirectory, new HashSet<string>(), _searchFilter);
		}
	}
}

