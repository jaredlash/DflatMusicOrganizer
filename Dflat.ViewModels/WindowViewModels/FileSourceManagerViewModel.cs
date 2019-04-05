﻿using Dflat.Business;
using Dflat.Business.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Specialized;
using Dflat.Business.Factories;

namespace Dflat.ViewModels
{
    public class FileSourceManagerViewModel : ViewModelBase, IViewModel
    {
        #region Private backing fields

        private FileSourceFolder selectedFileSourceFolder;
        private IUnitOfWorkLifetimeManager uowManager;

        #endregion


        #region Constructor

        public FileSourceManagerViewModel(IUnitOfWorkLifetimeManager uowManager)
        {
            this.FileSourceFolders = new ObservableCollection<FileSourceFolder>();
            this.uowManager = uowManager;

            ((ObservableCollection<FileSourceFolder>)FileSourceFolders).CollectionChanged += FileSourceFolders_Changed;
        }

        #endregion

        #region ViewModel Load

        public override void ViewModelInitialize()
        {
            base.ViewModelInitialize();

            LoadFileSourceFolders();
        }

        private void LoadFileSourceFolders()
        {
            FileSourceFolders.Clear();
            SelectedFileSourceFolder = null;

            foreach (var fileSourceFolder in uowManager.UnitOfWork.IFileSourceFolderRepository.GetAll())
                FileSourceFolders.Add(fileSourceFolder);

            // Add in some test file source folders
            FileSourceFolders.Add(new FileSourceFolder {
                Path = @"Z:\music\albums",
                IncludeInScans = true,
            });
            FileSourceFolders.Add(new FileSourceFolder {
                Path = @"C:\Users\Jared\Desktop",
                IncludeInScans = false,
            });
            FileSourceFolders.Add(new FileSourceFolder {
                Path = @"C:\Users\Jared\Some\Other\Really\Nested\Directory\With a possible long name\Somewhere",
                IncludeInScans = false,
            });
        }

        #endregion

        #region Override on close

        public override void ViewModelClose()
        {
            if (uowManager != null)
                uowManager.Dispose();

            base.ViewModelClose();
        }

        #endregion

        #region Event callbacks

        private void FileSourceFolders_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Count));
        }

        #endregion


        #region Commands

        public ICommand SaveCommand {
            get {
                return new RelayCommand(c => uowManager.UnitOfWork.SaveChanges(), p => uowManager.UnitOfWork.HasChanges());
            }
        }

        #endregion

        #region Public Properties

        public ICollection<FileSourceFolder> FileSourceFolders { get; private set; }

        public int Count {
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
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(CanEdit));
                NotifyPropertyChanged(nameof(SelectedFileSourceFolderExcludeCount));
            }
        }

        public int SelectedFileSourceFolderExcludeCount
        {
            get
            {
                return (SelectedFileSourceFolder != null ? SelectedFileSourceFolder.ExcludePaths.Count : 0);
            }
        }

        public bool CanEdit
        {
            get { return SelectedFileSourceFolder != null; }
        }

        #endregion
    }
}
