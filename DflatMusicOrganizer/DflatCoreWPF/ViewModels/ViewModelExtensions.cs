using Dflat.Application.Models;

namespace DflatCoreWPF.ViewModels;

public static class ViewModelExtensions
{
    public static void SetFromViewModel(this FileSourceFolder fileSourceFolder, FileSourceFolderEditorViewModel viewModel)
    {
        fileSourceFolder.FileSourceFolderID = viewModel.FileSourceFolderID;
        fileSourceFolder.Name = viewModel.Name;
        fileSourceFolder.Path = viewModel.Path;
        fileSourceFolder.IsTemporaryMedia = viewModel.IsTemporaryMedia;
        fileSourceFolder.LastScanStart = viewModel.LastScanStart;
        fileSourceFolder.IsChanged = viewModel.IsChanged;
        // Copy exclude paths
        fileSourceFolder.ExcludePaths.Clear();
        foreach (var excludePath in viewModel.ExcludePaths)
        {
            fileSourceFolder.ExcludePaths.Add(excludePath);
        }
    }
}
