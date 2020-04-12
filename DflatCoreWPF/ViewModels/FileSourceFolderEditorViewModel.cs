using Caliburn.Micro;
using Dflat.Application.Models;
using DflatCoreWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DflatCoreWPF.ViewModels
{
    public class FileSourceFolderEditorViewModel : Screen
    {
        private readonly IFolderChooserDialog folderChooserDialog;

        

        public FileSourceFolderEditorViewModel(IFolderChooserDialog folderChooserDialog)
        {
            this.folderChooserDialog = folderChooserDialog;
        }




        private int fileSourceFolderID;

        public int FileSourceFolderID
        {
            get { return fileSourceFolderID; }
            set
            {
                if (fileSourceFolderID != value)
                {
                    fileSourceFolderID = value;
                    IsChanged = true;
                }
                NotifyOfPropertyChange(() => FileSourceFolderID);
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    IsChanged = true;
                }
                NotifyOfPropertyChange(() => Name);
                NotifyOfPropertyChange(() => CanOkay);
            }
        }

        private string path;

        public string Path
        {
            get { return path; }
            set
            {
                if (path != value)
                {
                    path = value;
                    IsChanged = true;
                }
                NotifyOfPropertyChange(() => Path);
                NotifyOfPropertyChange(() => CanOkay);
            }
        }

        private bool isTemporaryMedia;

        public bool IsTemporaryMedia
        {
            get { return isTemporaryMedia; }
            set
            {
                if (isTemporaryMedia != value)
                {
                    isTemporaryMedia = value;
                    IsChanged = true;
                }
                NotifyOfPropertyChange(() => IsTemporaryMedia);
            }
        }

        private DateTime? lastScanStart;

        public DateTime? LastScanStart
        {
            get { return lastScanStart; }
            set
            {
                if (lastScanStart != value)
                {
                    lastScanStart = value;
                    IsChanged = true;
                }
                NotifyOfPropertyChange(() => LastScanStart);
            }
        }

        private bool isChanged;

        public bool IsChanged
        {
            get { return isChanged; }
            set
            {
                isChanged = value;
                NotifyOfPropertyChange(() => IsChanged);
            }
        }




        public void ChoosePath()
        {
            folderChooserDialog.InitialFolder = path;
            folderChooserDialog.Title = "Choose File Source Folder";
            var result = folderChooserDialog.ShowDialog();

            var newPath = folderChooserDialog.ResultFolder;

            
            if (result == true && !string.IsNullOrEmpty(newPath))
            {
                Path = newPath;
            }
        }


        public ObservableCollection<ExcludePath> ExcludePaths { get; set; } = new ObservableCollection<ExcludePath>();


        private ExcludePath selectedExcludePath;
        public ExcludePath SelectedExcludePath
        {
            get
            {
                return selectedExcludePath;
            }
            set
            {
                selectedExcludePath = value;
                NotifyOfPropertyChange(() => SelectedExcludePath);
                NotifyOfPropertyChange(() => CanRemoveExcludePath);
            }
        }



        public void AddExcludePath()
        {
            folderChooserDialog.InitialFolder = path;
            folderChooserDialog.Title = "Chooser Folder to exclude";
            var result = folderChooserDialog.ShowDialog();

            string newPath = folderChooserDialog.ResultFolder;
            

            if (result != true || string.IsNullOrEmpty(newPath))
                return;

            // Only add exclude folder if we don't already have it
            if (ExcludePaths.Any(p => p.Path == newPath))
                return;

            ExcludePaths.Add(new ExcludePath { FileSourceFolderID = FileSourceFolderID, Path = newPath });
            IsChanged = true;
        }

        public bool CanRemoveExcludePath => (selectedExcludePath != null);

        public void RemoveExcludePath()
        {
            if (selectedExcludePath == null)
                return;

            IsChanged = true;

            ExcludePaths.Remove(SelectedExcludePath);
            NotifyOfPropertyChange(() => CanOkay);
        }

        public async Task Cancel()
        {
            await TryCloseAsync(false);
        }


        public bool CanOkay
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Path) && !string.IsNullOrWhiteSpace(Name);
            }
        }

        public async Task Okay()
        {
            await TryCloseAsync(true);
        }

    }
}
