using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Dflat.Application.Models
{
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

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                CallPropertyChanged(nameof(Name));
            }
        }

        private string path;

        public string Path
        {
            get { return path; }
            set
            {
                path = value;
                CallPropertyChanged(nameof(Path));
            }
        }

        private bool isTemporaryMedia;

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


        private bool isChanged;

        public bool IsChanged
        {
            get { return isChanged; }
            set
            {
                isChanged = value;
                CallPropertyChanged(nameof(IsChanged));
            }
        }



        public ObservableCollection<ExcludePath> ExcludePaths { get; set; } = new ObservableCollection<ExcludePath>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void CallPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
