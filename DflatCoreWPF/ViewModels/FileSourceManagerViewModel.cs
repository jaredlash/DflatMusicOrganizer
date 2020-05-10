using AutoMapper;
using Dflat.Application.Models;
using Dflat.Application.Repositories;
using Dflat.Application.Services.JobServices;
using DflatCoreWPF.WindowService;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace DflatCoreWPF.ViewModels
{
    public class FileSourceManagerViewModel : ViewModelBase
    {
        #region Private backing fields

        private FileSourceFolder selectedFileSourceFolder;
        private bool enableOverlay;
        private readonly IFileSourceFolderRepository fileSourceFolderRepository;
        private readonly IJobService<FileSourceFolderScanJob> fileSourceFolderScanService;
        private readonly IMapper mapper;
        private readonly IWindowService windowService;
        private readonly ConfirmDialogViewModel confirmDialogViewModel;
        private readonly AlertDialogViewModel alertDialogViewModel;
        private readonly FileSourceFolderEditorViewModel sourceFolderEditorViewModel;

        private readonly List<FileSourceFolder> removedFolders;

        private readonly HashSet<FileSourceFolder> addedOrUpdatedFolders;
        #endregion


        #region Constructor

        public FileSourceManagerViewModel(IFileSourceFolderRepository fileSourceFolderRepository,
                                          IJobService<FileSourceFolderScanJob> fileSourceFolderScanService,
                                          IMapper mapper,
                                          IWindowService windowService,
                                          ConfirmDialogViewModel confirmDialogViewModel,
                                          AlertDialogViewModel alertDialogViewModel,
                                          FileSourceFolderEditorViewModel sourceFolderEditorViewModel)
        {
            this.FileSourceFolders = new BindingList<FileSourceFolder>();
            this.removedFolders = new List<FileSourceFolder>();
            this.addedOrUpdatedFolders = new HashSet<FileSourceFolder>();
            this.fileSourceFolderRepository = fileSourceFolderRepository;
            this.fileSourceFolderScanService = fileSourceFolderScanService;
            this.mapper = mapper;
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
            addedOrUpdatedFolders.Clear();
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

        public ICommand EditCommand { get => new RelayCommand(() => Edit(), CanEdit); }

        public ICommand RemoveCommand { get => new RelayCommand(() => Remove(), CanRemove); }

        public ICommand QueueFolderScanCommand { get => new RelayCommand(() => QueueFolderScan()); }

        public ICommand SaveCommand { get => new RelayCommand(async () => await Save(), CanSave); }

        public ICommand SaveSelectedCommand { get => new RelayCommand(async () => await SaveSelected()); }

        public ICommand ClosingCommand { get => new RelayCommand<CancelEventArgs>((e) => OnClosing(e)); }

        public ICommand CancelCommand { get => new RelayCommand(() => Cancel()); }

        #endregion


        #region Private methods
        private void Add()
        {
            var currentFolder = new FileSourceFolder();

            mapper.Map<FileSourceFolder, FileSourceFolderEditorViewModel>(currentFolder, sourceFolderEditorViewModel);
            sourceFolderEditorViewModel.IsChanged = currentFolder.IsChanged;
            bool? acceptChanges = windowService.ShowDialog(sourceFolderEditorViewModel);
            if (acceptChanges == true)
            {
                mapper.Map<FileSourceFolderEditorViewModel, FileSourceFolder>(sourceFolderEditorViewModel, currentFolder);
                FileSourceFolders.Add(currentFolder);
            }
            RaisePropertyChanged(() => HasChanges);
            RaisePropertyChanged(() => CanSave);
            RaisePropertyChanged(() => CanSaveSelected);
            (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            RaisePropertyChanged(() => CancelButtonText);
            RaisePropertyChanged(() => Count);
        }

        private void Edit()
        {
            var currentFolder = SelectedFileSourceFolder;

            mapper.Map<FileSourceFolder, FileSourceFolderEditorViewModel>(currentFolder, sourceFolderEditorViewModel);
            sourceFolderEditorViewModel.IsChanged = currentFolder.IsChanged;
            bool? acceptChanges = windowService.ShowDialog(sourceFolderEditorViewModel);
            if (acceptChanges == true)
            {
                mapper.Map<FileSourceFolderEditorViewModel, FileSourceFolder>(sourceFolderEditorViewModel, currentFolder);
            }
            RaisePropertyChanged(() => HasChanges);
            RaisePropertyChanged(() => CanSave);
            RaisePropertyChanged(() => CanSaveSelected);
            (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            RaisePropertyChanged(() => CancelButtonText);
        }

        private bool CanRemove => SelectedFileSourceFolder != null;
        private void Remove()
        {
            var currentFolder = SelectedFileSourceFolder;
            if (currentFolder == null)
                return;

            if (currentFolder.FileSourceFolderID != 0) removedFolders.Add(currentFolder);

            FileSourceFolders.Remove(currentFolder);
            RaisePropertyChanged(() => CanSave);
            RaisePropertyChanged(() => CanSaveSelected);
            (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
            RaisePropertyChanged(() => CancelButtonText);
            RaisePropertyChanged(() => HasChanges);
            RaisePropertyChanged(() => Count);
        }

        private void QueueFolderScan()
        {
            var currentFolder = SelectedFileSourceFolder;
            if (currentFolder == null)
                return;

            SubmitFolderScanJobRequest(currentFolder);
        }

        private async Task Save()
        {
            EnableOverlay = true;
            try
            {
                foreach (var folder in FileSourceFolders)
                {
                    if (folder.IsChanged)
                        addedOrUpdatedFolders.Add(folder);
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
            
            (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
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

            if (addedOrUpdatedFolders.Count > 0)
            {
                if (PromptToScanFolders() == true)
                {
                    foreach (var folder in addedOrUpdatedFolders)
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

