using System;
using System.Collections.Generic;
using System.IO;
using Infrastructure.IO.Interfaces.Filesystem;

namespace Infrastructure.IO.Filesystem
{
	public class FolderSearchService : IFolderSearchService
	{
        private readonly Predicate<string> _searchFilter;

		public FolderSearchService ()
		{
            _searchFilter = (String f) => true;
        }

        public FolderSearchService (Predicate<string> condition)
        {
            _searchFilter = condition;
        }

		public IFolderSearchServiceResult FindFiles (String sourceDirectory, Predicate<String> condition)
		{
			Stack<String> dirs = new Stack<String> (20);
			FolderSearchServiceResult result = new FolderSearchServiceResult ();

			if (Directory.Exists (sourceDirectory))
				dirs.Push (sourceDirectory);
			else
				result.ErrorLog.Add(String.Format("{0}: Directory does not exist: {1}", this.GetType().ToString(), sourceDirectory));

			while (dirs.Count > 0)
			{
				String currentDirectory = dirs.Pop ();
				String[] subDirectories;

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
					dirs.Push(str);

			}

			return result;
		}


		public IFolderSearchServiceResult FindFiles (String sourceDirectory)
		{
			return FindFiles(sourceDirectory, _searchFilter);
		}
	}
}

