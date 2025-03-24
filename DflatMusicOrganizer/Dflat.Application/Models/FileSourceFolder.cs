using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Dflat.Application.Models;

public class FileSourceFolder : INotifyPropertyChanged
{
    private int fileSourceFolderID;
    public int FileSourceFolderID
    {
        get { return fileSourceFolderID; }
        set
        {
            fileSourceFolderID = value;
            CallPropertyChanged(nameof(FileSourceFolderID));
        }
    }

    private string name = string.Empty;

    public string Name
    {
        get { return name; }
        set
        {
            name = value;
            CallPropertyChanged(nameof(Name));
        }
    }

    private string path = string.Empty;

    public string Path
    {
        get { return path; }
        set
        {
            path = value;
            CallPropertyChanged(nameof(Path));
        }
    }

    private bool isTemporaryMedia = false;
    public bool IsTemporaryMedia
    {
        get { return isTemporaryMedia; }
        set
        {
            isTemporaryMedia = value;
            CallPropertyChanged(nameof(IsTemporaryMedia));
        }
    }


    private DateTime? lastScanStart;
    public DateTime? LastScanStart
    {
        get { return lastScanStart; }
        set
        {
            lastScanStart = value;
            CallPropertyChanged(nameof(LastScanStart));
        }
    }


    private bool isChanged = false;
    public bool IsChanged
    {
        get { return isChanged; }
        set
        {
            isChanged = value;
            CallPropertyChanged(nameof(IsChanged));
        }
    }



    public ObservableCollection<ExcludePath> ExcludePaths { get; set; } = [];

    public event PropertyChangedEventHandler? PropertyChanged;

    private void CallPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

}
