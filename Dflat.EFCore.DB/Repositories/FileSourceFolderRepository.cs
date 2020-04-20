using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.EFCore.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace Dflat.EFCore.DB.Repositories
{
    public class FileSourceFolderRepository : IFileSourceFolderRepository
    {
        private readonly IMapper mapper;

        public FileSourceFolderRepository(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task<IEnumerable<Application.Models.FileSourceFolder>> GetAllAsync()
        {
            var result = new List<Application.Models.FileSourceFolder>();

            using (var context = new DataContext())
            {
                var fileSourceFolders = await context.FileSourceFolders.Include(f => f.ExcludePaths).ToListAsync();

                mapper.Map(fileSourceFolders, result);
            }

            foreach (var folder in result)
                folder.IsChanged = false;

            return result;
        }


        public IEnumerable<Application.Models.FileSourceFolder> GetAll()
        {
            var result = new List<Application.Models.FileSourceFolder>();

            using (var context = new DataContext())
            {
                var fileSourceFolders = context.FileSourceFolders.Include(f => f.ExcludePaths).ToList();

                mapper.Map(fileSourceFolders, result);
            }

            foreach (var folder in result)
                folder.IsChanged = false;

            return result;
        }


        public async Task<bool> UpdateAllAsync(IEnumerable<Application.Models.FileSourceFolder> fileSourceFolders)
        {
            using (var context = new DataContext())
            {

                // Remove any deleted fileSourceFolders and their exclude paths
                var currentFolders = await context.FileSourceFolders.ToListAsync();
                foreach (var current in currentFolders)
                {
                    if (fileSourceFolders.Any(f => f.FileSourceFolderID == current.FileSourceFolderID) == false)
                    {
                        current.ExcludePaths.Clear();
                        context.FileSourceFolders.Remove(current);
                    }
                }

                foreach (var source in fileSourceFolders)
                {
                    List<Models.ExcludePath> excludePaths = new List<Models.ExcludePath>();
                    foreach (var path in source.ExcludePaths)
                    {
                        excludePaths.Add(new Models.ExcludePath { ExcludePathID = path.ExcludePathID, Path = path.Path });
                    }

                    // Add
                    if (source.FileSourceFolderID == 0)
                    {
                        Models.FileSourceFolder fileSourceFolderData = new Models.FileSourceFolder
                        {
                            Path = source.Path,
                            Name = source.Name,
                            IsTemporaryMedia = source.IsTemporaryMedia,
                            LastScanStart = source.LastScanStart,
                            ExcludePaths = excludePaths
                        };

                        context.FileSourceFolders.Add(fileSourceFolderData);
                        try
                        {
                            _ = await context.SaveChangesAsync();
                            source.FileSourceFolderID = fileSourceFolderData.FileSourceFolderID;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else // Update
                    {
                        var fileSourceFolder = context.FileSourceFolders.Where( f => f.FileSourceFolderID == source.FileSourceFolderID).Include(f => f.ExcludePaths).FirstOrDefault();
                        if (fileSourceFolder == null)
                            continue;

                        fileSourceFolder.Path = source.Path;
                        fileSourceFolder.Name = source.Name;
                        fileSourceFolder.IsTemporaryMedia = source.IsTemporaryMedia;
                        fileSourceFolder.LastScanStart = source.LastScanStart;

                        // first take care of removed exclude paths
                        var toRemove = fileSourceFolder.ExcludePaths
                            .Where(p => excludePaths.Any(c => c.ExcludePathID == p.ExcludePathID) == false)
                            .ToList();

                        foreach (var p in toRemove)
                        {
                            fileSourceFolder.ExcludePaths.Remove(p);
                        }

                        // Now we add any new ones (exclude paths can only be added or removed, not updated)
                        foreach (var p in excludePaths.Where(p => p.ExcludePathID == 0))
                        {
                            fileSourceFolder.ExcludePaths.Add(p);
                        }

                    }
                }

                try
                {
                    _ = await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            // clear IsChanged flag to indicate object matches DB
            foreach (var source in fileSourceFolders)
                source.IsChanged = false;

            return true;
        }

        public Application.Models.FileSourceFolder Get(int fileSourceFolderID)
        {
            Application.Models.FileSourceFolder result = null;

            using (var context = new DataContext())
            {
                var fileSourceFolder = context.FileSourceFolders
                    .Where((f) => f.FileSourceFolderID == fileSourceFolderID)
                    .Include(f => f.ExcludePaths).FirstOrDefault();
                if (fileSourceFolder == null)
                    return null;

                result = mapper.Map<Application.Models.FileSourceFolder>(fileSourceFolder);
            }

            if (result != null) result.IsChanged = false;

            return result;
        }
    }
}
