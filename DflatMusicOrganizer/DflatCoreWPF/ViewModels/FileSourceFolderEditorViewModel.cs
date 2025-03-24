using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dflat.Application.Models;
using DflatCoreWPF.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DflatCoreWPF.ViewModels;

public partial class FileSourceFolderEditorViewModel : ViewModelBase
{
    private readonly IFolderChooserDialog folderChooserDialog;

    

    public FileSourceFolderEditorViewModel(IFolderChooserDialog folderChooserDialog)
    {
        this.folderChooserDialog = folderChooserDialog;
    }

    public void UseFileSourceFolder(FileSourceFolder fileSourceFolder)
    {
        FileSourceFolderID = fileSourceFolder.FileSourceFolderID;
        Name = fileSourceFolder.Name;
        Path = fileSourceFolder.Path;
        IsTemporaryMedia = fileSourceFolder.IsTemporaryMedia;
        LastScanStart = fileSourceFolder.LastScanStart;
        IsChanged = fileSourceFolder.IsChanged;
        // Copy exclude paths
        ExcludePaths = new ObservableCollection<ExcludePath>(fileSourceFolder.ExcludePaths);
    }

    #region Bindable properties

    [ObservableProperty]
    private int fileSourceFolderID;
    partial void OnFileSourceFolderIDChanging(int value)
    {
        IsChanged = true;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanOkay))]
    [NotifyCanExecuteChangedFor(nameof(OkayCommand))]
    private string name = string.Empty;
    partial void OnNameChanging(string value)
    {
        IsChanged = true;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanOkay))]
    [NotifyCanExecuteChangedFor(nameof(OkayCommand))]
    private string path = string.Empty;
    partial void OnPathChanging(string value)
    {
        IsChanged = true;
    }

    [ObservableProperty]
    private bool isTemporaryMedia;
    partial void OnIsTemporaryMediaChanging(bool value)
    {
        IsChanged = true;
    }

    [ObservableProperty]
    private DateTime? lastScanStart;
    partial void OnLastScanStartChanging(DateTime? value)
    {
        IsChanged = true;
    }

    [ObservableProperty]
    private bool isChanged;


    public ObservableCollection<ExcludePath> ExcludePaths { get; set; } = new ObservableCollection<ExcludePath>();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanRemoveExcludePath))]
    [NotifyCanExecuteChangedFor(nameof(RemoveExcludePathCommand))]
    private ExcludePath? selectedExcludePath = null;

    public bool CanRemoveExcludePath { get => (SelectedExcludePath != null); }

    public bool CanOkay { get => !string.IsNullOrWhiteSpace(Path) && !string.IsNullOrWhiteSpace(Name); }

    #endregion


    #region Private command methods
    [RelayCommand]
    private void ChoosePath()
    {
        folderChooserDialog.InitialFolder = Path;
        folderChooserDialog.Title = "Choose File Source Folder";
        var result = folderChooserDialog.ShowDialog();

        var newPath = folderChooserDialog.ResultFolder;


        if (result == true && !string.IsNullOrEmpty(newPath))
        {
            Path = newPath;
        }
    }

    [RelayCommand]
    private void AddExcludePath()
    {
        folderChooserDialog.InitialFolder = Path;
        folderChooserDialog.Title = "Chooser Folder to exclude";
        var result = folderChooserDialog.ShowDialog();

        string newPath = folderChooserDialog.ResultFolder;


        if (result != true || string.IsNullOrEmpty(newPath))
            return;

        // Only add exclude folder if we don't already have it
        if (ExcludePaths.Any(p => p.Path == newPath))
            return;

        ExcludePaths.Add(new ExcludePath { Path = newPath });
        IsChanged = true;
    }

    [RelayCommand]
    private void RemoveExcludePath()
    {
        if (SelectedExcludePath == null)
            return;

        IsChanged = true;

        ExcludePaths.Remove(SelectedExcludePath);
        OnPropertyChanged(nameof(CanOkay));
        OkayCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void Cancel()
    {
        TryClose(false);
    }

    [RelayCommand(CanExecute = nameof(CanOkay))]
    private void Okay()
    {
        TryClose(true);
    }


    #endregion
}
