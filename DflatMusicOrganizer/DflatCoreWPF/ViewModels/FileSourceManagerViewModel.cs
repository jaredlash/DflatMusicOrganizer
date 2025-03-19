using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.Application.Services.JobServices;
using DflatCoreWPF.WindowService;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DflatCoreWPF.ViewModels
{
    public class FileSourceManagerViewModel : ViewModelBase
    {
        #region Private backing fields

        private FileSourceFolder selectedFileSourceFolder;
        private bool enableOverlay;
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

            RaisePropertyChanged(() => Count);
        }

        #endregion


        #region Bindable Properties

        public BindingList<FileSourceFolder> FileSourceFolders { get; private set; }

        public int Count
        {
            get { return FileSourceFolders.Count; }
        }

        public FileSourceFolder SelectedFileSourceFolder
        {
            get
            {
                return selectedFileSourceFolder;
            }
            set
            {
                selectedFileSourceFolder = value;
                RaisePropertyChanged(() => SelectedFileSourceFolder);
                RaisePropertyChanged(() => CanEdit);
                RaisePropertyChanged(() => CanSaveSelected);
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RemoveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                RaisePropertyChanged(() => SelectedFileSourceFolderExcludeCount);
            }
        }

        public bool CanEdit { get => SelectedFileSourceFolder != null; }

        public bool EnableOverlay
        {
            get { return enableOverlay; }
            set
            {
                enableOverlay = value;
                RaisePropertyChanged(() => EnableOverlay);
            }
        }

        public string CancelButtonText
        {
            get { return HasChanges ? "Cancel" : "Close"; }
        }

        public int SelectedFileSourceFolderExcludeCount => (SelectedFileSourceFolder != null ? SelectedFileSourceFolder.ExcludePaths.Count : 0);

        public bool CanSave { get => HasChanges; }

        public bool CanSaveSelected { get => SelectedFileSourceFolder != null ? SelectedFileSourceFolder.IsChanged : false; }
        #endregion


        #region Commands
        public ICommand InitializeCommand { get => new RelayCommand(() => Initialize()); }

        public ICommand AddCommand { get => new RelayCommand(() => Add()); }

        public ICommand EditCommand { get => new RelayCommand(() => Edit()); }

        public ICommand RemoveCommand { get => new RelayCommand(() => Remove()); }

        public ICommand QueueFolderScanCommand { get => new RelayCommand(async () => await QueueFolderScan()); }

        public ICommand SaveCommand { get => new RelayCommand(async () => await Save()); }

        public ICommand SaveSelectedCommand { get => new RelayCommand(async () => await SaveSelected()); }

        public ICommand ClosingCommand { get => new RelayCommand<CancelEventArgs>((e) => OnClosing(e)); }

        public ICommand CancelCommand { get => new RelayCommand(() => Cancel()); }

        #endregion


        #region Private methods
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
            RaisePropertyChanged(() => HasChanges);
            RaisePropertyChanged(() => CanSave);
            RaisePropertyChanged(() => CanSaveSelected);
            RaisePropertyChanged(() => CancelButtonText);
            RaisePropertyChanged(() => Count);
        }

        private void Edit()
        {
            var currentFolder = SelectedFileSourceFolder;

            sourceFolderEditorViewModel.UseFileSourceFolder(currentFolder);
            bool? acceptChanges = windowService.ShowDialog(sourceFolderEditorViewModel);
            if (acceptChanges == true)
            {
                currentFolder.SetFromViewModel(sourceFolderEditorViewModel);
            }
            RaisePropertyChanged(() => HasChanges);
            RaisePropertyChanged(() => CanSave);
            RaisePropertyChanged(() => CanSaveSelected);
            RaisePropertyChanged(() => CancelButtonText);
        }

        private void Remove()
        {
            var currentFolder = SelectedFileSourceFolder;
            if (currentFolder == null)
                return;

            if (currentFolder.FileSourceFolderID != 0) removedFolders.Add(currentFolder);

            FileSourceFolders.Remove(currentFolder);
            RaisePropertyChanged(() => CanSave);
            RaisePropertyChanged(() => CanSaveSelected);
            RaisePropertyChanged(() => CancelButtonText);
            RaisePropertyChanged(() => HasChanges);
            RaisePropertyChanged(() => Count);
        }

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
            RaisePropertyChanged(() => CanSave);
            RaisePropertyChanged(() => CancelButtonText);

            EnableOverlay = false;
        }

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
            RaisePropertyChanged(() => CanSave);
            RaisePropertyChanged(() => CanSaveSelected);
            RaisePropertyChanged(() => CancelButtonText);
            RaisePropertyChanged(() => HasChanges);
        }


        private void OnClosing(CancelEventArgs args)
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

        private void Cancel()
        {
            TryClose();
        }

        private bool HasChanges => removedFolders.Count > 0 || FileSourceFolders.Any(f => f.IsChanged);

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

