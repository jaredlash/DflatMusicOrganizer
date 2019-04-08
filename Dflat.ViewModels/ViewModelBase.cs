

using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;

namespace Dflat.ViewModels
{
    
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase, IViewModel
    {
        public ICommand CloseCommand {
            get
            {
                return new RelayCommand(() => ViewModelClose());
            }
        }

        public event EventHandler OnClose;

        public virtual void ViewModelClose()
        {
            if (OnClose != null) OnClose.Invoke(this, null);
        }

        public ICommand InitializeCommand
        {
            get
            {
                return new RelayCommand(() => ViewModelInitialize());
            }
        }

        public virtual void ViewModelInitialize()
        {

        }
        
    }
}
