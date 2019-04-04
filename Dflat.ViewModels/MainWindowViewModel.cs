using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dflat.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        private readonly IViewService viewService;

        public MainWindowViewModel(IViewService viewService)
        {
            this.viewService = viewService;
        }


        #region public ICommands

        public ICommand OpenFileSourceManagerCommand {
            get
            {
                return new RelayCommand(p => OpenFileSourceManager());
            }
        }

        public ICommand OpenJobsViewCommand
        {
            get
            {
                return new RelayCommand(p => OpenJobsView());
            }
        }

        #endregion

        
        private void OpenFileSourceManager()
        {
            viewService.ShowViewModel<FileSourceManagerViewModel>();
        }

        private void OpenJobsView()
        {
            throw new NotImplementedException();
        }

    }
}
