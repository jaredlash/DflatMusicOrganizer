using Dflat.Application.Models;
using DflatCoreWPF.Utilities;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DflatCoreWPF.ViewModels
{
    public class FileSourceFolderEditorViewModel : ViewModelBase
    {
        private readonly IFolderChooserDialog folderChooserDialog;

        

        public FileSourceFolderEditorViewModel(IFolderChooserDialog folderChooserDialog)
        {
            this.folderChooserDialog = folderChooserDialog;
        }



        #region Bindable properties

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
                RaisePropertyChanged(() => FileSourceFolderID);
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
                RaisePropertyChanged(() => Name);
                RaisePropertyChanged(() => CanOkay);
                (OkayCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
                RaisePropertyChanged(() => Path);
                RaisePropertyChanged(() => CanOkay);
                (OkayCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
                RaisePropertyChanged(() => IsTemporaryMedia);
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
                RaisePropertyChanged(() => LastScanStart);
            }
        }

        private bool isChanged;

        public bool IsChanged
        {
            get { return isChanged; }
            set
            {
                isChanged = value;
                RaisePropertyChanged(() => IsChanged);
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
                RaisePropertyChanged(() => SelectedExcludePath);
                RaisePropertyChanged(() => CanRemoveExcludePath);
                (RemoveExcludePathCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool CanRemoveExcludePath { get => (selectedExcludePath != null); }

        public bool CanOkay { get => !string.IsNullOrWhiteSpace(Path) && !string.IsNullOrWhiteSpace(Name); }

        #endregion

        #region Commands

        public ICommand ChoosePathCommand { get => new RelayCommand(() => ChoosePath()); }

        public ICommand AddExcludePathCommand { get => new RelayCommand(() => AddExcludePath()); }
        
        public ICommand RemoveExcludePathCommand { get => new RelayCommand(() => RemoveExcludePath(), CanRemoveExcludePath); }

        public ICommand CancelCommand { get => new RelayCommand(() => Cancel()); }

        public ICommand OkayCommand { get => new RelayCommand(() => Okay(), CanOkay); }
        
        
        #endregion


        #region Private command methods
        private void ChoosePath()
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

        private void AddExcludePath()
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

            ExcludePaths.Add(new ExcludePath { Path = newPath });
            IsChanged = true;
        }

        private void RemoveExcludePath()
        {
            if (selectedExcludePath == null)
                return;

            IsChanged = true;

            ExcludePaths.Remove(SelectedExcludePath);
            RaisePropertyChanged(() => CanOkay);
            (OkayCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private void Cancel()
        {
            TryClose(false);
        }

        private void Okay()
        {
            TryClose(true);
        }


        #endregion
    }
}
