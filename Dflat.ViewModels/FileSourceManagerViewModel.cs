using Dflat.Business;
using Dflat.Business.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.Specialized;

namespace Dflat.ViewModels
{
    public class FileSourceManagerViewModel : ViewModelBase, IViewModel
    {
        private readonly IUnitOfWork unitOfWork;
        private FileSourceFolder selectedFileSourceFolder;

        

        public FileSourceManagerViewModel(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            this.FileSourceFolders = new ObservableCollection<FileSourceFolder>();
            ((ObservableCollection<FileSourceFolder>)FileSourceFolders).CollectionChanged += FileSourceFolders_Changed;
        }


        public override void ViewModelInitialize()
        {
            base.ViewModelInitialize();

            LoadFileSourceFolders();
        }

        private void LoadFileSourceFolders()
        {
            FileSourceFolders.Clear();
            SelectedFileSourceFolder = null;

            foreach (var fileSourceFolder in unitOfWork.IFileSourceFolderRepository.GetAll())
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

        public override void ViewModelClose()
        {
            unitOfWork.Dispose();
            base.ViewModelClose();
        }


        private void FileSourceFolders_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Count));
        }


        public ICommand SaveCommand {
            get {
                return new RelayCommand(c => unitOfWork.SaveChanges(), p => unitOfWork.HasChanges());
            }
        }

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
    }
}
