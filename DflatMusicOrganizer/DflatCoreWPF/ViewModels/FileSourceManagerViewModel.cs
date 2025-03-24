using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.Application.Services.JobServices;
using DflatCoreWPF.WindowService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DflatCoreWPF.ViewModels
{
    public partial class FileSourceManagerViewModel : ViewModelBase
    {
        #region Private backing fields

        
        private readonly IFileSourceFolderRepository fileSourceFolderRepository;
        private readonly IJobService<FileSourceFolderScanJob> fileSourceFolderScanService;
        private readonly IWindowService windowService;
        private readonly ConfirmDialogViewModel confirmDialogViewModel;
        private readonly AlertDialogViewModel alertDialogViewModel;
        private readonly FileSourceFolderEditorViewModel sourceFolderEditorViewModel;

        private readonly List<FileSourceFolder> removedFolders;

        private readonly HashSet<FileSourceFolder> changedFoldersToScan;
        #endregion


        #region Constructor

        public FileSourceManagerViewModel(IFileSourceFolderRepository fileSourceFolderRepository,
                                          IJobService<FileSourceFolderScanJob> fileSourceFolderScanService,
                                          IWindowService windowService,
                                          ConfirmDialogViewModel confirmDialogViewModel,
                                          AlertDialogViewModel alertDialogViewModel,
                                          FileSourceFolderEditorViewModel sourceFolderEditorViewModel)
        {
            this.FileSourceFolders = new BindingList<FileSourceFolder>();
            this.removedFolders = new List<FileSourceFolder>();
            this.changedFoldersToScan = new HashSet<FileSourceFolder>();
            this.fileSourceFolderRepository = fileSourceFolderRepository;
            this.fileSourceFolderScanService = fileSourceFolderScanService;
            this.windowService = windowService;
            this.confirmDialogViewModel = confirmDialogViewModel;
            this.alertDialogViewModel = alertDialogViewModel;
            this.sourceFolderEditorViewModel = sourceFolderEditorViewModel;

            this.EnableOverlay = false;
        }

        #endregion

        #region ViewModel Load

        [RelayCommand]
        public void Initialize()
        {
            FileSourceFolders.Clear();
            removedFolders.Clear();
            changedFoldersToScan.Clear();
            SelectedFileSourceFolder = null;


            LoadFileSourceFolders();
            this.EnableOverlay = false;
        }


        private void LoadFileSourceFolders()
        {
            IEnumerable<FileSourceFolder> sources = new List<FileSourceFolder>();

            try
            {
                // TODO: Make async
                sources = fileSourceFolderRepository.GetAll();
            }
            catch (Exception ex)
            {
                alertDialogViewModel.Title = "Problem loading source folders"; ;
                alertDialogViewModel.Message = $"A problem was encountered when loading the source folders: {ex.Message}";
                windowService.ShowDialog(alertDialogViewModel);
            }


            foreach (var fileSourceFolder in sources)
                FileSourceFolders.Add(fileSourceFolder);

            OnPropertyChanged(nameof(Count));
        }

        #endregion


        #region Bindable Properties

        public BindingList<FileSourceFolder> FileSourceFolders { get; private set; }

        public int Count
        {
            get { return FileSourceFolders.Count; }
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanEdit))]
        [NotifyCanExecuteChangedFor(nameof(EditCommand))]
        [NotifyPropertyChangedFor(nameof(CanSaveSelected))]
        [NotifyCanExecuteChangedFor(nameof(SaveSelectedCommand))]
        [NotifyPropertyChangedFor(nameof(SelectedFileSourceFolderExcludeCount))]
        private FileSourceFolder selectedFileSourceFolder;

        public bool CanEdit { get => SelectedFileSourceFolder != null; }

        [ObservableProperty]
        private bool enableOverlay;

        public string CancelButtonText
        {
            get { return HasChanges ? "Cancel" : "Close"; }
        }

        public int SelectedFileSourceFolderExcludeCount => (SelectedFileSourceFolder != null ? SelectedFileSourceFolder.ExcludePaths.Count : 0);

        public bool CanSave { get => HasChanges; }

        public bool CanSaveSelected { get => SelectedFileSourceFolder != null ? SelectedFileSourceFolder.IsChanged : false; }

        private bool HasChanges => removedFolders.Count > 0 || FileSourceFolders.Any(f => f.IsChanged);
        #endregion


        #region Private methods
        [RelayCommand]
        private void Add()
        {
            var currentFolder = new FileSourceFolder();

            sourceFolderEditorViewModel.UseFileSourceFolder(currentFolder);
            bool? acceptChanges = windowService.ShowDialog(sourceFolderEditorViewModel);
            if (acceptChanges == true)
            {
                currentFolder.SetFromViewModel(sourceFolderEditorViewModel);
                FileSourceFolders.Add(currentFolder);
            }
            OnPropertyChanged(nameof(HasChanges));;
            OnPropertyChanged(nameof(CanSave));
            SaveCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanSaveSelected));
            SaveSelectedCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CancelButtonText));
            OnPropertyChanged(nameof(Count));
        }

        [RelayCommand]
        private void Edit()
        {
            var currentFolder = SelectedFileSourceFolder;

            sourceFolderEditorViewModel.UseFileSourceFolder(currentFolder);
            bool? acceptChanges = windowService.ShowDialog(sourceFolderEditorViewModel);
            if (acceptChanges == true)
            {
                currentFolder.SetFromViewModel(sourceFolderEditorViewModel);
            }
            OnPropertyChanged(nameof(HasChanges));
            OnPropertyChanged(nameof(CanSave));
            SaveCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanSaveSelected));
            SaveSelectedCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CancelButtonText));
        }

        [RelayCommand]
        private void Remove()
        {
            var currentFolder = SelectedFileSourceFolder;
            if (currentFolder == null)
                return;

            if (currentFolder.FileSourceFolderID != 0) removedFolders.Add(currentFolder);

            FileSourceFolders.Remove(currentFolder);
            OnPropertyChanged(nameof(CanSave));
            SaveCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanSaveSelected));
            SaveSelectedCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CancelButtonText));
            OnPropertyChanged(nameof(HasChanges));
            OnPropertyChanged(nameof(Count));
        }

        [RelayCommand]
        private async Task QueueFolderScan()
        {
            var currentFolder = SelectedFileSourceFolder;
            if (currentFolder == null)
                return;

            if (currentFolder.IsChanged)
            {
                if (PromptUnsavedScanSelected() == true)
                    await SaveSelected();
                else
                    return;
            }
            SubmitFolderScanJobRequest(currentFolder);
            changedFoldersToScan.Remove(currentFolder); // Do not queue a job again unless we make additional changes
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task Save()
        {
            EnableOverlay = true;
            try
            {
                foreach (var folder in FileSourceFolders)
                {
                    if (folder.IsChanged)
                        changedFoldersToScan.Add(folder);
                }
                await fileSourceFolderRepository.UpdateAllAsync(FileSourceFolders);
                removedFolders.Clear();
            }
            catch (Exception ex)
            {
                alertDialogViewModel.Title = "Problem saving changes"; ;
                alertDialogViewModel.Message = $"Problem saving changes: {ex.Message}";
                windowService.ShowDialog(alertDialogViewModel);
            }
            OnPropertyChanged(nameof(CanSave));
            SaveCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CancelButtonText));

            EnableOverlay = false;
        }

        [RelayCommand(CanExecute = nameof(CanSaveSelected))]
        private async Task SaveSelected()
        {
            var folder = SelectedFileSourceFolder;
            if (folder == null)
                return;

            try
            {
                await fileSourceFolderRepository.AddOrUpdateAsync(folder);
            }
            catch (Exception ex)
            {
                alertDialogViewModel.Title = "Problem saving changes"; ;
                alertDialogViewModel.Message = $"Problem saving changes: {ex.Message}";
                windowService.ShowDialog(alertDialogViewModel);
            }
            changedFoldersToScan.Add(folder);  // Individually saved folders should be scanned at the end too
            OnPropertyChanged(nameof(CanSave));
            SaveCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanSaveSelected));
            SaveSelectedCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CancelButtonText));
            OnPropertyChanged(nameof(HasChanges));
        }

        [RelayCommand]
        private void Closing(CancelEventArgs args)
        {
            if (HasChanges)
            {
                if (PromptUnsavedChanges() != true)
                {
                    args.Cancel = true;
                    return;
                }
            }

            if (changedFoldersToScan.Count > 0)
            {
                if (PromptToScanFolders() == true)
                {
                    foreach (var folder in changedFoldersToScan)
                        SubmitFolderScanJobRequest(folder);
                }
            }
            args.Cancel = false;
        }

        private bool? PromptUnsavedChanges()
        {
            confirmDialogViewModel.Title = "Discard unsaved Changes?";
            confirmDialogViewModel.Message = "There are unsaved changes. Click Cancel to go back and Save.";
            confirmDialogViewModel.NoText = "Cancel";
            confirmDialogViewModel.YesText = "Close";
            var result = windowService.ShowDialog(confirmDialogViewModel);
            return result;
        }

        private bool? PromptUnsavedScanSelected()
        {
            confirmDialogViewModel.Title = "Save changes to File Source Folder to scan";
            confirmDialogViewModel.Message = "There are unsaved changes to this File Source Folder. Save changes and continue?";
            confirmDialogViewModel.NoText = "Cancel";
            confirmDialogViewModel.YesText = "Save";
            var result = windowService.ShowDialog(confirmDialogViewModel);
            return result;
        }

        private bool? PromptToScanFolders()
        {
            confirmDialogViewModel.Title = "Scan updated folders?";
            confirmDialogViewModel.Message = "File Source folders have been changed.  Would you like to scan the changed folders?";
            confirmDialogViewModel.NoText = "No";
            confirmDialogViewModel.YesText = "Yes";
            var result = windowService.ShowDialog(confirmDialogViewModel);
            return result;
        }

        [RelayCommand]
        private void Cancel()
        {
            TryClose();
        }


        private void SubmitFolderScanJobRequest(FileSourceFolder folder)
        {
            fileSourceFolderScanService.SubmitJobRequest(
                new FileSourceFolderScanJob
                {
                    FileSourceFolderID = folder.FileSourceFolderID,
                    Description = $"Scan of '{folder.Name}' {folder.Path}"
                });
        }
        #endregion
    }
}

